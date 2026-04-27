using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Configuration;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Infrastructure.Persistence;
using InventoryManagement.Infrastructure.Services.AI.Plugins;

namespace InventoryManagement.Infrastructure.Services.AI;

public class SemanticKernelCopilotService : IAiCopilotService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;

    public SemanticKernelCopilotService(IConfiguration config, ApplicationDbContext context)
    {
        // 1. Pull the keys from your appsettings.Development.json
        var endpoint = config["LocalAi:Endpoint"]!;
        var modelId = config["LocalAi:ModelId"]!;

        // 2. Build the AI Brain
        var builder = Kernel.CreateBuilder();
        
        // 3. Give the AI Brain access to your C# Database functions!
        var customHttpClient = new HttpClient { BaseAddress = new Uri(endpoint) };
        builder.AddOpenAIChatCompletion(
            modelId: modelId,
            apiKey: "ollama", 
            httpClient: customHttpClient);
        builder.Plugins.AddFromObject(new WarehousePlugin(context));

        _kernel = builder.Build();
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string> AskWarehouseQuestionAsync(string userPrompt, string userId)
    {
        // 1. Set the AI's personality
        var history = new ChatHistory("You are an expert warehouse management AI. You help users query their inventory safely. Be concise, professional, and helpful.");
        
        // 2. Add the user's question
        history.AddUserMessage(userPrompt);

        // 3. CRITICAL: Tell the AI it has permission to trigger your C# Plugin!
        var executionSettings = new OpenAIPromptExecutionSettings 
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        // 4. Send it to Azure and wait for the response
        var result = await _chatCompletionService.GetChatMessageContentAsync(history, executionSettings, _kernel);
        
        return result.Content ?? "I'm sorry, I couldn't process that request.";
    }

    public Task<string> ProcessVoiceCommandAsync(byte[] audioStream)
    {
        // We will build this out when we attach the Speech service later!
        throw new NotImplementedException();
    }
}