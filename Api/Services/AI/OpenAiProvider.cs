using Microsoft.Extensions.Options;
using OpenAI.Chat;
using LegalStrategyAdvisor.Api.Configuration;
using LegalStrategyAdvisor.Api.Services.AI;

namespace LegalStrategyAdvisor.Api.Services.AI;

public class OpenAiProvider : IAiProvider
{
    private readonly OpenAIOptions _options;
    private readonly ChatClient _chatClient;
    private readonly ILogger<OpenAiProvider> _logger;

    public string ProviderName => "OpenAI";

    public OpenAiProvider(IOptions<AiProviderOptions> options, ILogger<OpenAiProvider> logger)
    {
        _options = options.Value.OpenAI;
        _logger = logger;

        if (string.IsNullOrEmpty(_options.ApiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured");
        }

        try
        {
            var client = new OpenAI.OpenAIClient(_options.ApiKey);
            _chatClient = client.GetChatClient(_options.Model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize OpenAI client");
            throw new AiException(ProviderName, "Failed to initialize OpenAI client", ex);
        }
    }

    public async Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Sending request to OpenAI with model: {Model}", _options.Model);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a legal strategy advisor AI assistant. Provide comprehensive, well-structured legal analysis and strategy recommendations. Always include relevant legal precedents, potential risks, and actionable steps. Format your response clearly with sections and bullet points where appropriate."),
                new UserChatMessage(prompt)
            };

            var chatCompletionOptions = new ChatCompletionOptions
            {
                MaxOutputTokenCount = _options.MaxTokens,
                Temperature = (float)_options.Temperature
            };

            var response = await _chatClient.CompleteChatAsync(messages, chatCompletionOptions, cancellationToken);

            if (response?.Value?.Content?.Count > 0)
            {
                var content = string.Join("\n", response.Value.Content.Select(c => c.Text));

                _logger.LogInformation("OpenAI request completed successfully. Tokens used: {TokensUsed}",
                    response.Value.Usage?.TotalTokenCount ?? 0);

                return content;
            }

            throw new AiException(ProviderName, "Empty response received from OpenAI");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("OpenAI request was cancelled");
            throw;
        }
        catch (Exception ex) when (ex is not AiException)
        {
            _logger.LogError(ex, "Error generating response from OpenAI");
            throw new AiException(ProviderName, $"OpenAI request failed: {ex.Message}", ex);
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Test with a minimal request
            var testMessage = new UserChatMessage("Hello");
            var testOptions = new ChatCompletionOptions { MaxOutputTokenCount = 10 };

            var response = await _chatClient.CompleteChatAsync([testMessage], testOptions, cancellationToken);
            return response?.Value != null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OpenAI availability check failed");
            return false;
        }
    }
}
