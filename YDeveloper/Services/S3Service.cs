using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace YDeveloper.Services
{
    public interface IS3Service
    {
        Task<string> UploadFileAsync(string bucketName, string key, Stream fileStream, string contentType);
        Task<string> UploadFileAsync(string bucketName, string key, string content, string contentType);
        Task<string> UploadImageAsync(string bucketName, string key, Stream imageStream);
    }

    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;

        public S3Service(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<string> UploadFileAsync(string bucketName, string key, Stream fileStream, string contentType)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = fileStream,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            };

            await _s3Client.PutObjectAsync(request);

            // Return Public URL
            return $"https://{bucketName}.s3.amazonaws.com/{key}";
        }

        public async Task<string> UploadFileAsync(string bucketName, string key, string content, string contentType)
        {
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
            {
                return await UploadFileAsync(bucketName, key, stream, contentType);
            }
        }

        public async Task<string> UploadImageAsync(string bucketName, string key, Stream imageStream)
        {
            try
            {
                // 1. Reset stream position if needed
                if (imageStream.CanSeek) imageStream.Position = 0;

                // 2. Load Image (Detects format automatically)
                using var image = await Image.LoadAsync(imageStream);

                // 3. Convert to WebP
                using var outputStream = new MemoryStream();
                await image.SaveAsync(outputStream, new WebpEncoder());
                outputStream.Position = 0;

                // 4. Change extension to .webp
                string webpKey = Path.ChangeExtension(key, ".webp");

                // 5. Upload Optimized Image
                return await UploadFileAsync(bucketName, webpKey, outputStream, "image/webp");
            }
            catch
            {
                // Fallback: If not an image or error, upload original
                if (imageStream.CanSeek) imageStream.Position = 0;
                return await UploadFileAsync(bucketName, key, imageStream, "application/octet-stream");
            }
        }
    }
}
