using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;

namespace YDeveloper.ViewComponents
{
    public class DynamicHomepageViewComponent : ViewComponent
    {
        private readonly YDeveloperContext _context;

        public DynamicHomepageViewComponent(YDeveloperContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var content = await _context.ContentItems
                .Where(c => c.Section == "homepage")
                .ToDictionaryAsync(c => c.Key, c => c.Value);

            var features = await _context.ContentItems
                .Where(c => c.Section == "features" && c.IsActive)
                .OrderBy(c => c.Order)
                .ToListAsync();

            var pricingPackages = await _context.PricingPackages
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();

            ViewBag.Content = content;
            ViewBag.Features = features;
            ViewBag.PricingPackages = pricingPackages;

            return View();
        }
    }
}
