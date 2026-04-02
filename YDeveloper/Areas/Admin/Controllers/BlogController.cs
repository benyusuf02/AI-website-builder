using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;
using YDeveloper.Services;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Moderator")]
    public class BlogController : Controller
    {
        private readonly YDeveloperContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IS3Service _s3Service;
        private readonly IConfiguration _configuration;
        private readonly IImageOptimizationService _imageService;

        public BlogController(
            YDeveloperContext context,
            UserManager<ApplicationUser> userManager,
            IS3Service s3Service,
            IConfiguration configuration,
            IImageOptimizationService imageService)
        {
            _context = context;
            _userManager = userManager;
            _s3Service = s3Service;
            _configuration = configuration;
            _imageService = imageService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.BlogPosts.Include(b => b.Author).OrderByDescending(b => b.CreatedAt).ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPost post, IFormFile? coverImage)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                post.AuthorId = user?.Id;
                post.CreatedAt = DateTime.UtcNow;

                // Slug generation if empty
                if (string.IsNullOrEmpty(post.Slug))
                {
                    post.Slug = GenerateSlug(post.Title);
                }

                // Handle Image
                // Handle Image
                if (coverImage != null)
                {
                    var bucketName = _configuration["AWS:BucketName"];
                    if (!string.IsNullOrEmpty(bucketName))
                    {
                        try
                        {
                            var (optimizedStream, optimizedFileName) = await _imageService.OptimizeToStreamAsync(coverImage);
                            using (optimizedStream)
                            {
                                var key = $"blog/{Guid.NewGuid()}_{optimizedFileName}";
                                post.CoverImageUrl = await _s3Service.UploadFileAsync(bucketName, key, optimizedStream, "image/webp");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Fallback or Log
                            // If optimization fails, maybe upload original? Or just fail. 
                            // Let's log and rethrow or handle. For now, strict fail is safer than broken image.
                            ModelState.AddModelError("CoverImageUrl", "Resim optimizasyonu başarısız oldu: " + ex.Message);
                            return View(post);
                        }
                    }
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Yazı oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null) return NotFound();
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogPost post, IFormFile? coverImage)
        {
            if (id != post.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPost = await _context.BlogPosts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingPost == null) return NotFound();

                    // Preserve fields not in form
                    post.AuthorId = existingPost.AuthorId;
                    post.CreatedAt = existingPost.CreatedAt;

                    if (coverImage != null)
                    {
                        var bucketName = _configuration["AWS:BucketName"];
                        if (!string.IsNullOrEmpty(bucketName))
                        {
                            var (optimizedStream, optimizedFileName) = await _imageService.OptimizeToStreamAsync(coverImage);
                            using (optimizedStream)
                            {
                                var key = $"blog/{Guid.NewGuid()}_{optimizedFileName}";
                                post.CoverImageUrl = await _s3Service.UploadFileAsync(bucketName, key, optimizedStream, "image/webp");
                            }
                        }
                    }
                    else
                    {
                        post.CoverImageUrl = existingPost.CoverImageUrl;
                    }

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Yazı güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(post.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post != null)
            {
                _context.BlogPosts.Remove(post);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Yazı silindi.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }

        private string GenerateSlug(string title)
        {
            return title.ToLower()
                .Replace(" ", "-")
                .Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c")
                .Replace(".", "").Replace("?", "").Replace("!", "");
        }
    }
}
