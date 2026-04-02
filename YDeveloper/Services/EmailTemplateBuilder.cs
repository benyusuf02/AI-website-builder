using System;

namespace YDeveloper.Services
{
    public static class EmailTemplateBuilder
    {
        public static string Build(string title, string bodyContent, string? actionButtonText = null, string? actionButtonUrl = null)
        {
            string buttonHtml = "";
            if (!string.IsNullOrEmpty(actionButtonText) && !string.IsNullOrEmpty(actionButtonUrl))
            {
                buttonHtml = $@"
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{actionButtonUrl}' style='background-color: #3498db; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold; font-family: sans-serif;'>
                            {actionButtonText}
                        </a>
                    </div>";
            }

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; }}
        .header {{ background-color: #2c3e50; padding: 20px; text-align: center; color: #ffffff; }}
        .content {{ padding: 30px; color: #333333; line-height: 1.6; }}
        .footer {{ background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; color: #7f8c8d; }}
        a {{ color: #3498db; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin: 0; font-size: 24px;'>YDeveloper</h1>
        </div>
        <div class='content'>
            <h2 style='color: #2c3e50; margin-top: 0;'>{title}</h2>
            <p>{bodyContent}</p>
            {buttonHtml}
            <p style='margin-top: 30px; border-top: 1px solid #eee; padding-top: 20px; font-size: 14px; color: #999;'>
                Bu işlemi siz yapmadıysanız, lütfen bu e-postayı dikkate almayın.
            </p>
        </div>
        <div class='footer'>
            &copy; {DateTime.Now.Year} YDeveloper Yazılım Çözümleri.<br>
            Bu otomatik bir mesajdır, lütfen cevaplamayınız.
        </div>
    </div>
</body>
</html>";
        }
    }
}
