namespace YDeveloper.Models.Settings
{
    public class EmailTemplate
    {
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
    }

    public static class EmailTemplates
    {
        public static EmailTemplate Welcome => new()
        {
            TemplateName = "Welcome",
            Subject = "YDeveloper'a Hoş Geldiniz!",
            Body = @"
                <h1>Merhaba {{UserName}},</h1>
                <p>YDeveloper ailesine hoş geldiniz!</p>
                <p>Sitenizi oluşturmaya başlamak için hazırsınız.</p>
                <a href='{{DashboardUrl}}'>Dashboard'a Git</a>
            "
        };

        public static EmailTemplate PasswordReset => new()
        {
            TemplateName = "PasswordReset",
            Subject = "Şifre Sıfırlama Talebi",
            Body = @"
                <h1>Şifre Sıfırlama</h1>
                <p>Şifrenizi sıfırlamak için aşağıdaki linke tıklayın:</p>
                <a href='{{ResetUrl}}'>Şifremi Sıfırla</a>
                <p>Bu linkin geçerlilik süresi 1 saattir.</p>
            "
        };

        public static EmailTemplate PaymentSuccess => new()
        {
            TemplateName = "PaymentSuccess",
            Subject = "Ödemeniz Alındı",
            Body = @"
                <h1>Ödeme Başarılı</h1>
                <p>{{Amount}} TL tutarındaki ödemeniz alınmıştır.</p>
                <p>Paket: {{PackageName}}</p>
                <p>Fatura için dashboard'unuzu kontrol edin.</p>
            "
        };
    }
}
