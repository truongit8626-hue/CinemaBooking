using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CinemaBooking.Controllers
{
    public class GeminiTestController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public GeminiTestController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var apiKey = _configuration["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return Content("❌ Không đọc được Gemini API Key từ User Secrets.");
            }

            var client = _httpClientFactory.CreateClient();

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = "Hãy trả lời ngắn gọn bằng tiếng Việt: CinemaBooking là gì?"
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://generativelanguage.googleapis.com/v1beta/models/gemini-3.6-flash:generateContent");

            request.Headers.Add("x-goog-api-key", apiKey);
            request.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await client.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return Content(
                    $"❌ Gemini API lỗi.\n\nStatus: {response.StatusCode}\n\n{responseContent}",
                    "text/plain; charset=utf-8");
            }

            using var document = JsonDocument.Parse(responseContent);

            var text = document.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return Content(
                $"✅ Kết nối Gemini thành công!\n\nGemini trả lời:\n{text}",
                "text/plain; charset=utf-8");
        }
    }
}