using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace CinemaBooking.Services.AI
{
    public class GeminiService : IGeminiService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public GeminiService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GenerateAsync(
            string systemPrompt,
            string userPrompt)
        {
            var apiKey = _configuration["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "Không tìm thấy Gemini API Key.");
            }

            if (string.IsNullOrWhiteSpace(systemPrompt))
            {
                throw new ArgumentException(
                    "System prompt không được để trống.",
                    nameof(systemPrompt));
            }

            if (string.IsNullOrWhiteSpace(userPrompt))
            {
                throw new ArgumentException(
                    "User prompt không được để trống.",
                    nameof(userPrompt));
            }

            // Giới hạn kích thước prompt để tránh gửi dữ liệu quá lớn.
            const int maxPromptLength = 100_000;

            if (systemPrompt.Length > maxPromptLength ||
                userPrompt.Length > maxPromptLength)
            {
                throw new InvalidOperationException(
                    "Dữ liệu gửi đến AI quá dài. " +
                    "Vui lòng giảm lượng dữ liệu và thử lại.");
            }

            var requestBody = new
            {
                systemInstruction = new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = systemPrompt
                        }
                    }
                },
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = userPrompt
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash-lite:generateContent");

            // Giữ HTTP/1.1 vì đây là cấu hình đã test thành công
            // với Gemini API trong project.
            request.Version = HttpVersion.Version11;
            request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;

            request.Headers.Add("x-goog-api-key", apiKey);

            request.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var client = _httpClientFactory.CreateClient();

            // Timeout tối đa cho một request AI.
            client.Timeout = TimeSpan.FromSeconds(100);

            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(request);
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException(
                    "Gemini không phản hồi trong thời gian cho phép.");
            }
            catch (HttpRequestException)
            {
                throw new HttpRequestException(
                    "Không thể kết nối đến dịch vụ Gemini.");
            }

            var responseContent =
                await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new HttpRequestException(
                    "Gemini đang giới hạn số lượng yêu cầu. " +
                    "Vui lòng thử lại sau.",
                    null,
                    HttpStatusCode.TooManyRequests);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    "Gemini hiện không thể xử lý yêu cầu. " +
                    "Vui lòng thử lại sau.",
                    null,
                    response.StatusCode);
            }

            if (string.IsNullOrWhiteSpace(responseContent))
            {
                throw new InvalidOperationException(
                    "Gemini không trả về dữ liệu.");
            }

            try
            {
                using var document =
                    JsonDocument.Parse(responseContent);

                if (!document.RootElement.TryGetProperty(
                        "candidates",
                        out var candidates) ||
                    candidates.ValueKind != JsonValueKind.Array ||
                    candidates.GetArrayLength() == 0)
                {
                    throw new InvalidOperationException(
                        "Gemini không trả về kết quả hợp lệ.");
                }

                var candidate = candidates[0];

                if (!candidate.TryGetProperty(
                        "content",
                        out var content) ||
                    !content.TryGetProperty(
                        "parts",
                        out var parts) ||
                    parts.ValueKind != JsonValueKind.Array ||
                    parts.GetArrayLength() == 0)
                {
                    throw new InvalidOperationException(
                        "Gemini không trả về nội dung hợp lệ.");
                }

                var part = parts[0];

                if (!part.TryGetProperty(
                        "text",
                        out var textElement))
                {
                    throw new InvalidOperationException(
                        "Gemini không trả về nội dung văn bản.");
                }

                var text = textElement.GetString();

                if (string.IsNullOrWhiteSpace(text))
                {
                    throw new InvalidOperationException(
                        "Gemini không trả về nội dung.");
                }

                return text.Trim();
            }
            catch (JsonException)
            {
                throw new InvalidOperationException(
                    "Gemini trả về dữ liệu không đúng định dạng.");
            }
        }
    }
}