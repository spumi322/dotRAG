using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using dotRAG.API.Application;

namespace dotRAG.API.Infrastructure.LLM;

internal sealed class AnthropicLlmService : ILlmService
{
    private readonly string _apiKey;

    public AnthropicLlmService(IConfiguration config)
    {
        _apiKey = config["ApiKeys:AnthropicApiKey"]
            ?? throw new InvalidOperationException("ApiKeys:AnthropicApiKey not configured.");
    }

    public async Task<string> CompleteAsync(string prompt, CancellationToken ct = default)
    {
        using var client = new AnthropicClient(new APIAuthentication(_apiKey));
        var result = await client.Messages.GetClaudeMessageAsync(new MessageParameters
        {
            Model     = AnthropicModels.Claude45Haiku,
            MaxTokens = 1024,
            Messages  = [new Message(RoleType.User, prompt)]
        }, ct);
        return result.Message.ToString();
    }
}
