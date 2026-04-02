using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Models
{
    public class SystemSetting
    {
        [Key]
        [MaxLength(50)]
        public string Key { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
