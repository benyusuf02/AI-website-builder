using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Areas.Admin.Models; // Ensure this namespace exists
using YDeveloper.Data;
using YDeveloper.Models;
using YDeveloper.Services;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuditService _auditService;
        private readonly ILogger<UsersController> _logger;
        private readonly YDeveloperContext _context;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IAuditService auditService,
            ILogger<UsersController> logger,
            YDeveloperContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _auditService = auditService;
            _logger = logger;
            _context = context;
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Index(string search, string role = "All", string status = "All")
        {
            var query = _userManager.Users.AsQueryable();

            // 1. Search Filter
            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                query = query.Where(u => (u.Email != null && u.Email.ToLower().Contains(s)) ||
                                         (u.FullName != null && u.FullName.ToLower().Contains(s)) ||
                                         (u.BusinessName != null && u.BusinessName.ToLower().Contains(s)));
            }

            // 2. Status Filter
            if (status == "Active")
            {
                query = query.Where(u => u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow);
            }
            else if (status == "Banned")
            {
                query = query.Where(u => u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow);
            }

            // 3. Role Filter (Requires Join or Subquery)
            if (!string.IsNullOrEmpty(role) && role != "All")
            {
                var targetRole = await _roleManager.FindByNameAsync(role);
                if (targetRole != null)
                {
                    // Get User IDs in this role
                    var userIdsInRole = _context.UserRoles
                        .Where(ur => ur.RoleId == targetRole.Id)
                        .Select(ur => ur.UserId);

                    query = query.Where(u => userIdsInRole.Contains(u.Id));
                }
            }

            // Execute Main Query
            var users = await query.OrderByDescending(u => u.CreatedAt).Take(100).ToListAsync();
            var userIds = users.Select(u => u.Id).ToList();

            // Optimized Data Fetching (Prevent N+1)
            // Fetch Site Counts
            var siteCounts = await _context.Sites
                .Where(s => userIds.Contains(s.UserId))
                .GroupBy(s => s.UserId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count);

            // Fetch User Roles
            var userRolesMap = await _context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId))
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, RoleName = r.Name })
                .ToListAsync();

            var roleLookup = userRolesMap
                .GroupBy(x => x.UserId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.RoleName!).ToList());

            var model = users.Select(user => new AdminUserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "No Email",
                FullName = user.FullName ?? user.UserName ?? "Unknown",
                CreatedAt = user.CreatedAt,
                IsBanned = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow,
                LockoutEnd = user.LockoutEnd,
                Roles = roleLookup.ContainsKey(user.Id) ? roleLookup[user.Id] : new List<string>(),
                SiteCount = siteCounts.ContainsKey(user.Id) ? siteCounts[user.Id] : 0,
                WebsiteLimit = user.WebsiteLimit,
                AdminNote = user.AdminNote
            }).ToList();

            // Pass Filters to View
            ViewData["CurrentSearch"] = search;
            ViewData["CurrentRole"] = role;
            ViewData["CurrentStatus"] = status;

            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var viewModel = new AdminUserDetailViewModel
            {
                User = user,
                Roles = roles.ToList(),
                Sites = await _context.Sites.Where(s => s.UserId == id).OrderByDescending(s => s.Id).ToListAsync(),
                Transactions = await _context.PaymentTransactions.Where(t => t.UserId == id).OrderByDescending(t => t.Timestamp).Take(20).ToListAsync(),
                AuditLogs = await _context.AuditLogs.Where(l => l.PerformerUserId == id || l.TargetUserId == id).OrderByDescending(l => l.Timestamp).Take(50).ToListAsync(),
                Tickets = await _context.Tickets.Where(t => t.CreatorId == id).OrderByDescending(t => t.CreatedAt).Take(10).ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLimit(string userId, int limit)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (limit < 0) limit = 0;

            user.WebsiteLimit = limit;
            await _userManager.UpdateAsync(user);
            await _auditService.LogAsync(GetAdminId(), "UpdateLimit", $"User limit updated to {limit}", userId);

            TempData["Success"] = "Web sitesi limiti güncellendi.";
            return RedirectToAction(nameof(Details), new { id = userId });
        }

        [HttpPost]
        public async Task<IActionResult> Promote(string userId, string role)
        {
            if (string.IsNullOrEmpty(role) || (role != "Admin" && role != "Moderator"))
            {
                TempData["Error"] = "Geçersiz rol.";
                return RedirectToAction(nameof(Details), new { id = userId });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (!await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.AddToRoleAsync(user, role);
                await _auditService.LogAsync(GetAdminId(), "PromoteUser", $"User promoted to {role}", userId);
                TempData["Success"] = $"Kullanıcıya {role} yetkisi verildi.";
            }

            return RedirectToAction(nameof(Details), new { id = userId });
        }

        [HttpPost]
        public async Task<IActionResult> Demote(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);
                await _auditService.LogAsync(GetAdminId(), "DemoteUser", $"User removed from {role}", userId);
                TempData["Success"] = $"Kullanıcıdan {role} yetkisi alındı.";
            }

            return RedirectToAction(nameof(Details), new { id = userId });
        }

        [HttpPost]
        public async Task<IActionResult> Ban(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            await _auditService.LogAsync(GetAdminId(), "BanUser", "User banned indefinitely", userId);

            TempData["Success"] = "Kullanıcı yasaklandı.";
            return RedirectToAction(nameof(Details), new { id = userId });
        }

        [HttpPost]
        public async Task<IActionResult> Unban(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, null);
            await _auditService.LogAsync(GetAdminId(), "UnbanUser", "User unbanned", userId);

            TempData["Success"] = "Yasak kaldırıldı.";
            return RedirectToAction(nameof(Details), new { id = userId });
        }

        [HttpPost]
        public async Task<IActionResult> Impersonate(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["Error"] = "Diğer yöneticilerin hesabına giriş yapılamaz.";
                return RedirectToAction(nameof(Details), new { id = userId });
            }

            var adminId = GetAdminId();
            await _auditService.LogAsync(adminId, "ImpersonateUser", $"Admin impersonated user {user.Email}", user.Id);

            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, isPersistent: false);

            _logger.LogWarning($"Admin {adminId} impersonated user {user.Email}");

            return Redirect("~/");
        }

        [HttpPost]
        public async Task<IActionResult> SaveNote(string userId, string note)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.AdminNote = note;
            await _userManager.UpdateAsync(user);
            // No Audit Log for notes to keep it clean, or optional.
            TempData["Success"] = "Not kaydedildi.";
            return RedirectToAction(nameof(Details), new { id = userId });
        }

        [HttpPost]
        public async Task<IActionResult> RevokeSession(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.UpdateSecurityStampAsync(user);
            await _auditService.LogAsync(GetAdminId(), "RevokeSession", "User session revoked", userId);

            TempData["Success"] = "Kullanıcının tüm oturumları sonlandırıldı.";
            return RedirectToAction(nameof(Details), new { id = userId });
        }

        private string GetAdminId()
        {
            return _userManager.GetUserId(User) ?? "system";
        }
    }
}
