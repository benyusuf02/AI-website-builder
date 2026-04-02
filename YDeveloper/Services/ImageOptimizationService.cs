using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace YDeveloper.Services
{
    public class ImageOptimizationService : IImageOptimizationService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ImageOptimizationService> _logger;

        public ImageOptimizationService(IWebHostEnvironment env, ILogger<ImageOptimizationService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<string> OptimizeAndSaveAsync(IFormFile file, string outputFolder, int maxWidth = 1920, int quality = 80)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Dosya boş olamaz.");

            try
            {
                // Ensure output directory exists (wwwroot/outputFolder)
                var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadPath = Path.Combine(webRootPath, outputFolder);

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Generate new filename with .webp extension
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var newFileName = $"{fileName}_{Guid.NewGuid().ToString().Substring(0, 8)}.webp";
                var filePath = Path.Combine(uploadPath, newFileName);

                // Check dependencies (Assuming ImageSharp is installed as checked)
                using (var image = await Image.LoadAsync(file.OpenReadStream()))
                {
                    // Resize if larger than maxWidth
                    if (image.Width > maxWidth)
                    {
                        var newHeight = (int)((double)image.Height / image.Width * maxWidth);
                        image.Mutate(x => x.Resize(maxWidth, newHeight));
                    }

                    // Save as WebP
                    var encoder = new WebpEncoder { Quality = quality };
                    await image.SaveAsWebpAsync(filePath, encoder);
                }

                // Return relative path (e.g., "/images/blog/photo.webp")
                return $"/{outputFolder.TrimStart('/').Replace('\\', '/')}/{newFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resim optimizasyonu sırasında hata oluştu: {FileName}", file.FileName);
                throw; // Re-throw or handle as needed. For now, we prefer to know if it fails.
            }
        }

        public async Task<(Stream Stream, string FileName)> OptimizeToStreamAsync(IFormFile file, int maxWidth = 1920, int quality = 80)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Dosya boş olamaz.");

            try
            {
                var memoryStream = new MemoryStream();

                // Load Image
                using (var image = await Image.LoadAsync(file.OpenReadStream()))
                {
                    // Resize
                    if (image.Width > maxWidth)
                    {
                        var newHeight = (int)((double)image.Height / image.Width * maxWidth);
                        image.Mutate(x => x.Resize(maxWidth, newHeight));
                    }

                    // Save as WebP to MemoryStream
                    var encoder = new WebpEncoder { Quality = quality };
                    await image.SaveAsWebpAsync(memoryStream, encoder);
                }

                memoryStream.Position = 0; // Reset position for reading

                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var newFileName = $"{fileName}_{Guid.NewGuid().ToString().Substring(0, 8)}.webp";

                return (memoryStream, newFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resim stream optimizasyonu sırasında hata oluştu: {FileName}", file.FileName);
                throw;
            }
        }
    }
}
