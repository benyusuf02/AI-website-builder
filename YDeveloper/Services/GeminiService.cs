using Microsoft.DotNet.MSIdentity.Shared;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using YDeveloper.Constants;

namespace YDeveloper.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
            _apiKey = _configuration[AppConstants.GeminiApiKeyConfig] ?? throw new InvalidOperationException("Gemini API Key not configured");
        }
        public async Task<string> GenerateHtmlAsync(string prompt, string pageType = "Inner")
        {
            string roleDescription = pageType == "Landing"
                ? "Your task is to generate a high-quality, modern, and fully responsive SINGLE-PAGE LANDING PAGE."
                : "Your task is to generate a specific INNER PAGE (e.g. About, Contact, Services). Do NOT create a full landing page. Focus on the specific content requested.";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = $@"You are an elite Senior UI/UX Designer and Frontend Developer. 
{roleDescription}

**CRITICAL OUTPUT RULES:**
1.  **NO MARKDOWN:** Do NOT use ```html or any markdown syntax. Start directly with `<!DOCTYPE html>`.
2.  **COMPLETE FILE:** Output a valid, self-contained `index.html` file.
3.  **LIBRARIES (MANDATORY):** 
    -   **Bootstrap 5:** Use CDN: `<link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css"" rel=""stylesheet"">`
    -   **Bootstrap Icons:** Use CDN: `<link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.1/font/bootstrap-icons.css"">`
    -   **Google Fonts:** Use 'Outfit' or 'Inter': `<link href=""https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;600;700&display=swap"" rel=""stylesheet"">`
    -   **JavaScript:** Include Bootstrap Bundle JS at the end of body: `<script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js""></script>`
4.  **IMAGES:** Use high-quality Unsplash source URLs. **IMPORTANT:** ALL images MUST have descriptive `alt` tags for SEO.

**SEO & STRUCTURE REQUIREMENTS (CRITICAL):**
-   **Meta Tags:** You MUST include `<title>` (Business Name | Keyword) and `<meta name=""description"" content=""..."">` in the head.
-   **Semantic HTML:** Use `<header>`, `<nav>`, `<main>`, `<section>`, `<article>`, `<footer>` tags appropriately.
-   **Headings:** Use ONE `<h1>` for the main hero title. Use `<h2>` and `<h3>` for section headings.
-   **Content:** Write unique, engaging copy relevant to the client's industry. Do not use 'Lorem Ipsum'.

**DESIGN GUIDELINES:**
-   **Aesthetics:** The design must look PREMIUM, EXPENSIVE, and TRUSTWORTHY.
-   **Style:** Modern, clean, 'Glassmorphism' touches, ample whitespace (padding/margin), rounded corners (`rounded-4`).
-   **Colors:** Use a professional palette relevant to the industry in the prompt.
-   **Responsiveness:** Must look perfect on mobile. Use Bootstrap grid (`col-md-6`, `col-lg-4`).
-   **Navigation:** You MUST use `<ul class=""navbar-nav ms-auto"" id=""site-menu"">` for the main menu items.
-   **Forms:** If generating a Contact Form, set `<form action=""https://ydeveloper.com/Public/SubmitForm"" method=""POST"">` and include `<input type=""hidden"" name=""siteId"" value=""__SITE_ID__"">`.

**CLIENT CONTENT BRIEF:**
{prompt}

**EXECUTE:** Generate the strictly valid HTML code now. NO conversational text." }

                        }
    }

}

            };
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var modelUrl = "https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key=";
            var response = await _httpClient.PostAsync(modelUrl + _apiKey, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return $"<h1>Hata: Yapay zeka şu an cevap veremiyor. Status: {response.StatusCode}. Details: {errorBody}</h1>";
            }
            var responseString = await response.Content.ReadAsStringAsync();
            var jsonresponse = JsonNode.Parse(responseString);

            try
            {
                var text = jsonresponse?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
                if (!string.IsNullOrEmpty(text))
                {
                    text = text.Replace("```html", "").Replace("```", "");
                }
                return text ?? "<h1> AI Boş Döndü</h1>";
            }
            catch
            {
                return "<h1>AI cevabı işlenirken hata oluştu.</h1>";
            }
        }

        public async Task<string> GenerateContentAsync(string prompt)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var modelUrl = "https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash-exp:generateContent?key=";
            var response = await _httpClient.PostAsync(modelUrl + _apiKey, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                return "AI analiz şu an kullanılamıyor.";
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonresponse = JsonNode.Parse(responseString);

            try
            {
                var text = jsonresponse?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
                return text ?? "AI içgörü oluşturulamadı.";
            }
            catch
            {
                return "AI cevabı işlenirken hata oluştu.";
            }
        }
    }
}