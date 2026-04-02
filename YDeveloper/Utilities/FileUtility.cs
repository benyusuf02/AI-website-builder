namespace YDeveloper.Utilities
{
    public static class FileUtility
    {
        public static string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName).ToLowerInvariant();
        }

        public static bool IsImageFile(string fileName)
        {
            var ext = GetFileExtension(fileName);
            return new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" }.Contains(ext);
        }

        public static bool IsDocumentFile(string fileName)
        {
            var ext = GetFileExtension(fileName);
            return new[] { ".pdf", ".doc", ".docx", ".txt", ".xlsx", ".pptx" }.Contains(ext);
        }

        public static string GenerateUniqueFileName(string originalFileName)
        {
            var ext = GetFileExtension(originalFileName);
            return $"{Guid.NewGuid()}{ext}";
        }

        public static long GetFileSizeInMb(long bytes)
        {
            return bytes / (1024 * 1024);
        }
    }
}
