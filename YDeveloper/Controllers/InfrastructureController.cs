using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;

namespace YDeveloper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfrastructureController : ControllerBase
    {
        private readonly YDeveloperContext _context;

        public InfrastructureController(YDeveloperContext context)
        {
            _context = context;
        }

        // GET: api/infrastructure/check-domain?domain=foo.com
        [HttpGet("check-domain")]
        public async Task<IActionResult> CheckDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain)) return BadRequest();

            // 1. Host header validation for internal proxy (optional but good practice)
            // if (Request.Host.Host != "localhost") return Unauthorized();

            // 2. Check if we have this domain in our DB
            // Domain can be "ahmet.ydeveloper.com" (Subdomain) or "musteri.com" (CustomDomain)

            // Subdomain check
            if (domain.EndsWith(".ydeveloper.com"))
            {
                // We approve all subdomains for now, or check DB
                var subdomain = domain.Replace(".ydeveloper.com", "");
                var exists = await _context.Sites.AnyAsync(s => s.Subdomain == subdomain);
                if (exists) return Ok();
            }

            // Custom Domain check
            var site = await _context.Sites.FirstOrDefaultAsync(s => s.Domain == domain);

            if (site != null)
            {
                return Ok(); // we know this domain, issue cert
            }

            return NotFound(); // don't issue cert for unknown domains
        }
    }
}
