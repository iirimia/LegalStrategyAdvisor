using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using LegalStrategyAdvisor.Api.Configuration;
using LegalStrategyAdvisor.Api.Services.AI;
using System.ClientModel;

namespace LegalStrategyAdvisor.Api.Services.AI;

public class AzureOpenAiProvider : IAiProvider
{
    private readonly AzureOpenAIOptions _options;
    private readonly ChatClient _chatClient;
    private readonly ILogger<AzureOpenAiProvider> _logger;

    public string ProviderName => "AzureOpenAI";

    public AzureOpenAiProvider(IOptions<AiProviderOptions> options, ILogger<AzureOpenAiProvider> logger)
    {
        _options = options.Value.Azure;
        _logger = logger;

        if (string.IsNullOrEmpty(_options.ApiKey) || string.IsNullOrEmpty(_options.Endpoint))
        {
            throw new InvalidOperationException("Azure OpenAI API key and endpoint are required");
        }

        try
        {
            var client = new AzureOpenAIClient(new Uri(_options.Endpoint), new ApiKeyCredential(_options.ApiKey));
            _chatClient = client.GetChatClient(_options.DeploymentName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Azure OpenAI client");
            throw new AiException(ProviderName, "Failed to initialize Azure OpenAI client", ex);
        }
    }

    public async Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Sending request to Azure OpenAI with deployment: {Deployment}", _options.DeploymentName);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a legal strategy advisor AI assistant. Provide comprehensive, well-structured legal analysis and strategy recommendations. Always include relevant legal precedents, potential risks, and actionable steps. Format your response clearly with sections and bullet points where appropriate."),
                new UserChatMessage(prompt)
            };

            var chatCompletionOptions = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 2000,
                Temperature = 0.7f
            };

            var response = await _chatClient.CompleteChatAsync(messages, chatCompletionOptions, cancellationToken);

            if (response?.Value?.Content?.Count > 0)
            {
                var content = string.Join("\n", response.Value.Content.Select(c => c.Text));

                _logger.LogInformation("Azure OpenAI request completed successfully. Tokens used: {TokensUsed}",
                    response.Value.Usage?.TotalTokenCount ?? 0);

                return content;
            }

            throw new AiException(ProviderName, "Empty response received from Azure OpenAI");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Azure OpenAI request was cancelled");
            throw;
        }
        catch (ClientResultException ex)
        {
            _logger.LogError(ex, "Azure OpenAI request failed with status: {Status}", ex.Status);
            throw new AiException(ProviderName, $"Azure OpenAI request failed: {ex.Message}", ex.Status);
        }
        catch (Exception ex) when (ex is not AiException)
        {
            _logger.LogError(ex, "Error generating response from Azure OpenAI");
            throw new AiException(ProviderName, $"Azure OpenAI request failed: {ex.Message}", ex);
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
            _logger.LogWarning(ex, "Azure OpenAI availability check failed");
            return false;
        }
    }
}
