using Microsoft.AspNetCore.Mvc;

namespace YDeveloper.Controllers
{
    public class AssetController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public AssetController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile files)
        {
            try
            {
                if (files == null || files.Length == 0)
                    return BadRequest("Dosya Seçilemei");
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueName = Guid.NewGuid().ToString() + "_" + files.FileName;
                var filepatch = Path.Combine(uploadsFolder, uniqueName);

                using (var stream = new FileStream(filepatch, FileMode.Create))
                {
                    await files.CopyToAsync(stream);
                }
                var imageUrl = "/uploads/" + uniqueName;
                return Ok(new { data = new[] { imageUrl } });

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Yükleme Hatası" + ex.Message);
            }
        }
        [HttpGet]
        public IActionResult Load()
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                return Ok(new List<string>());
            var images = Directory.GetFiles(uploadsFolder)
            .Select(f => "/uploads/" + Path.GetFileName(f))
            .ToList();

            return Ok(images);
        }
    }
}
