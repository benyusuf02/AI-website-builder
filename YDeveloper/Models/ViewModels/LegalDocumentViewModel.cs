using System;

namespace YDeveloper.Models.ViewModels
{
    public class LegalDocumentViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string ContractId { get; set; } = string.Empty; // e.g. "TOS-2025-V1"

        // User Info
        public string FullName { get; set; } = "Ziyaretçi";
        public string Email { get; set; } = "Belirtilmemiş";
        public string IpAddress { get; set; } = "0.0.0.0";
        public string BusinessName { get; set; } = "Bireysel";
        public string Address { get; set; } = "-";

        // System Info
        public DateTime GeneratedDate { get; set; } = DateTime.Now;
        public string CompanyName { get; set; } = "yaptık.com Teknoloji A.Ş.";
        public string CompanyAddress { get; set; } = "Teknopark İstanbul";
        public string CompanyContact { get; set; } = "destek@yaptik.com";
    }
}
