namespace LegalStrategyAdvisor.Api.Services.AI;

public interface IAiProvider
{
    Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
    string ProviderName { get; }
}

public class AiRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string SystemPrompt { get; set; } = string.Empty;
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 2000;
}

public class AiResponse
{
    public string Content { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public bool IsFromFallback { get; set; }
}

public class AiException : Exception
{
    public string Provider { get; }
    public int? StatusCode { get; }

    public AiException(string provider, string message) : base(message)
    {
        Provider = provider;
    }

    public AiException(string provider, string message, Exception innerException) : base(message, innerException)
    {
        Provider = provider;
    }

    public AiException(string provider, string message, int statusCode) : base(message)
    {
        Provider = provider;
        StatusCode = statusCode;
    }
}
