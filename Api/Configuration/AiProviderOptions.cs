namespace LegalStrategyAdvisor.Api.Configuration;

public class AiProviderOptions
{
    public const string SectionName = "AiProvider";

    public string Provider { get; set; } = "Mock";
    public bool EnableFallback { get; set; } = true;
    public int MaxRetries { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 30;
    public OpenAIOptions OpenAI { get; set; } = new();
    public AzureOpenAIOptions Azure { get; set; } = new();
    public PythonAIOptions PythonAI { get; set; } = new();
    public RateLimitingOptions RateLimiting { get; set; } = new();
}

public class OpenAIOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4o-mini";
    public int MaxTokens { get; set; } = 2000;
    public double Temperature { get; set; } = 0.7;
    public string BaseUrl { get; set; } = "https://api.openai.com/v1";
}

public class AzureOpenAIOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = "2024-06-01";
}

public class PythonAIOptions
{
    public string BaseUrl { get; set; } = "http://localhost:8001";
    public int TimeoutSeconds { get; set; } = 30;
}

public class RateLimitingOptions
{
    public int RequestsPerMinute { get; set; } = 60;
    public int TokensPerMinute { get; set; } = 100000;
}

public enum AiProviderType
{
    Mock,
    OpenAI,
    AzureOpenAI,
    PythonAI
}
