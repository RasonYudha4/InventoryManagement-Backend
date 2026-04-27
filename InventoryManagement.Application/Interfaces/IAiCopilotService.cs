public interface IAiCopilotService
{
    Task<string> AskWarehouseQuestionAsync(string userPrompt, string userId);
    
    Task<string> ProcessVoiceCommandAsync(byte[] audioStream);
}