using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YDeveloper.Data;
using YDeveloper.Models;
using YDeveloper.Services;

namespace YDeveloper.Controllers
{
    [Authorize]
    public class PageController : Controller
    {
        private readonly IPageService _pageService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly YDeveloperContext _context;

        public PageController(IPageService pageService, UserManager<ApplicationUser> userManager, YDeveloperContext context)
        {
            _pageService = pageService;
            _userManager = userManager;
            _context = context; // Still needed for read-only View queries if not in service
        }

        [HttpGet]
        public IActionResult Index(int siteId)
        {
            var userId = _userManager.GetUserId(User);
            var site = _context.Sites.FirstOrDefault(s => s.Id == siteId && s.UserId == userId);
            if (site == null) return RedirectToAction("Index", "Dashboard");

            var pages = _context.Pages.Where(p => p.SiteId == siteId).ToList();
            ViewBag.SiteId = siteId;
            ViewBag.Domain = site.Domain;
            return View(pages);
        }

        [HttpGet]
        public IActionResult Create(int siteId)
        {
            var userId = _userManager.GetUserId(User);
            var site = _context.Sites.FirstOrDefault(s => s.Id == siteId && s.UserId == userId);
            if (site == null) return RedirectToAction("Index", "Dashboard");

            ViewBag.Site = site;
            return View(new Page { SiteId = siteId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(int siteId, string pageName, string slug, string prompt)
        {
            var userId = _userManager.GetUserId(User);
            try
            {
                await _pageService.CreatePageAsync(siteId, userId, pageName, slug, prompt);
                return RedirectToAction("Index", new { siteId = siteId });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", new { siteId = siteId }); // Or show error
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var page = await _pageService.GetPageAsync(id, userId);
            if (page == null) return NotFound();
            return View(page);
        }

        [HttpGet]
        public async Task<IActionResult> Design(int id)
        {
            var userId = _userManager.GetUserId(User);
            var page = await _pageService.GetPageAsync(id, userId);
            if (page == null) return NotFound();
            return View(page);
        }

        [HttpPost]
        public async Task<IActionResult> Save(int pageId, string htmlContent)
        {
            var userId = _userManager.GetUserId(User);
            var success = await _pageService.UpdateContentAsync(pageId, userId, htmlContent);
            if (success) return Ok();
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var page = await _pageService.GetPageAsync(id, userId);
            if (page == null) return RedirectToAction("Index", "Dashboard");

            int siteId = page.SiteId;
            await _pageService.DeletePageAsync(id, userId);
            return RedirectToAction("Index", new { siteId = siteId });
        }

        [HttpPost]
        public async Task<IActionResult> Publish(int pageId, string? htmlContent)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            try
            {
                string url = await _pageService.PublishPageAsync(pageId, userId, htmlContent);
                return Ok(new { success = true, url = url, message = "Yayınlandı." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
