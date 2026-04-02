using Microsoft.AspNetCore.Http;

namespace YDeveloper.Services
{
    public interface IImageOptimizationService
    {
        /// <summary>
        /// Optimizes an uploaded image (resizes if large, converts to WebP) and saves it.
        /// </summary>
        /// <param name="file">The uploaded file.</param>
        /// <param name="outputFolder">Relative folder path (e.g. "images/blog").</param>
        /// <param name="maxWidth">Maximum width (default 1920).</param>
        /// <param name="quality">WebP quality (default 80).</param>
        /// <returns>Relative URL to the saved image.</returns>
        Task<string> OptimizeAndSaveAsync(IFormFile file, string outputFolder, int maxWidth = 1920, int quality = 80);

        /// <summary>
        /// Optimizes an uploaded image and returns it as a MemoryStream (ready for S3 upload).
        /// </summary>
        /// <returns>Tuple of (Stream, FileName with .webp extension)</returns>
        Task<(Stream Stream, string FileName)> OptimizeToStreamAsync(IFormFile file, int maxWidth = 1920, int quality = 80);
    }
}
