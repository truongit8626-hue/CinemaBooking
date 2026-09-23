namespace CinemaBooking.Services.AI
{
    public interface IGeminiService
    {
        Task<string> GenerateAsync(
            string systemPrompt,
            string userPrompt);
    }
}