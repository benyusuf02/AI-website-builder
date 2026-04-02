using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Attributes
{
    /// <summary>
    /// File size validation attribute
    /// </summary>
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSizeInMb;

        public MaxFileSizeAttribute(int maxFileSizeInMb)
        {
            _maxFileSizeInMb = maxFileSizeInMb;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var maxSizeInBytes = _maxFileSizeInMb * 1024 * 1024;
                if (file.Length > maxSizeInBytes)
                {
                    return new ValidationResult($"Dosya boyutu {_maxFileSizeInMb}MB'dan büyük olamaz.");
                }
            }
            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Allowed file extensions validation
    /// </summary>
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(params string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_extensions.Contains(extension))
                {
                    return new ValidationResult($"Sadece şu dosya türlerine izin verilir: {string.Join(", ", _extensions)}");
                }
            }
            return ValidationResult.Success;
        }
    }
}
