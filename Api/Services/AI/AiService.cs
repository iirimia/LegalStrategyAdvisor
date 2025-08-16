using Microsoft.Extensions.Options;
using LegalStrategyAdvisor.Api.Configuration;
using LegalStrategyAdvisor.Api.Services.AI;
using System.Diagnostics;

namespace LegalStrategyAdvisor.Api.Services.AI;

public interface IAiService
{
    Task<AiResponse> GenerateStrategyAsync(string caseDescription, CancellationToken cancellationToken = default);
    Task<AiResponse> GenerateAnalysisAsync(AiRequest request, CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}

public class AiService : IAiService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AiProviderOptions _options;
    private readonly ILogger<AiService> _logger;
    private readonly List<IAiProvider> _providers;

    public AiService(
        IServiceProvider serviceProvider,
        IOptions<AiProviderOptions> options,
        ILogger<AiService> logger)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _logger = logger;
        _providers = [];

        InitializeProviders();
    }

    private void InitializeProviders()
    {
        try
        {
            // Primary provider based on configuration
            if (Enum.TryParse<AiProviderType>(_options.Provider, ignoreCase: true, out var primaryType))
            {
                var primaryProvider = CreateProvider(primaryType);
                if (primaryProvider != null)
                {
                    _providers.Add(primaryProvider);
                    _logger.LogInformation("Initialized primary AI provider: {Provider}", primaryType);
                }
            }

            // Add fallback providers if enabled
            if (_options.EnableFallback)
            {
                AddFallbackProviders(primaryType);
            }

            if (_providers.Count == 0)
            {
                _logger.LogWarning("No AI providers available, adding mock provider as fallback");
                var mockProvider = _serviceProvider.GetRequiredService<MockAiProvider>();
                _providers.Add(mockProvider);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing AI providers");
            // Ensure we always have at least the mock provider
            var mockProvider = _serviceProvider.GetRequiredService<MockAiProvider>();
            _providers.Clear();
            _providers.Add(mockProvider);
        }
    }

    private void AddFallbackProviders(AiProviderType primaryType)
    {
        var fallbackTypes = new List<AiProviderType>
        {
            AiProviderType.PythonAI,
            AiProviderType.OpenAI,
            AiProviderType.AzureOpenAI,
            AiProviderType.Mock
        };

        foreach (var type in fallbackTypes.Where(t => t != primaryType))
        {
            try
            {
                var provider = CreateProvider(type);
                if (provider != null)
                {
                    _providers.Add(provider);
                    _logger.LogInformation("Added fallback AI provider: {Provider}", type);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to initialize fallback provider: {Provider}", type);
            }
        }
    }

    private IAiProvider? CreateProvider(AiProviderType type)
    {
        return type switch
        {
            AiProviderType.PythonAI => _serviceProvider.GetRequiredService<PythonAiProvider>(),
            AiProviderType.OpenAI => CreateOpenAiProvider(),
            AiProviderType.AzureOpenAI => CreateAzureOpenAiProvider(),
            AiProviderType.Mock => _serviceProvider.GetRequiredService<MockAiProvider>(),
            _ => null
        };
    }

    private IAiProvider? CreateOpenAiProvider()
    {
        if (string.IsNullOrEmpty(_options.OpenAI.ApiKey))
        {
            _logger.LogDebug("OpenAI API key not configured, skipping provider");
            return null;
        }

        try
        {
            return _serviceProvider.GetRequiredService<OpenAiProvider>();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to create OpenAI provider");
            return null;
        }
    }

    private IAiProvider? CreateAzureOpenAiProvider()
    {
        if (string.IsNullOrEmpty(_options.Azure.ApiKey) || string.IsNullOrEmpty(_options.Azure.Endpoint))
        {
            _logger.LogDebug("Azure OpenAI not fully configured, skipping provider");
            return null;
        }

        try
        {
            return _serviceProvider.GetRequiredService<AzureOpenAiProvider>();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to create Azure OpenAI provider");
            return null;
        }
    }

    public async Task<AiResponse> GenerateStrategyAsync(string caseDescription, CancellationToken cancellationToken = default)
    {
        var systemPrompt = @"You are an expert legal strategy advisor AI. Your role is to provide comprehensive legal analysis and strategic recommendations.

Guidelines:
- Provide structured, actionable legal strategies
- Include relevant legal precedents and case law
- Identify potential risks and mitigation strategies  
- Suggest next steps and action items
- Use professional legal terminology appropriately
- Format responses with clear sections and bullet points
- Always include disclaimers about the need for qualified legal counsel

Focus areas:
- Case analysis and legal theory
- Procedural strategy and timeline
- Evidence gathering and preservation
- Risk assessment and mitigation
- Settlement considerations
- Litigation strategy if applicable";

        var request = new AiRequest
        {
            Prompt = $"Analyze this legal matter and provide a comprehensive strategy:\n\n{caseDescription}",
            SystemPrompt = systemPrompt,
            Temperature = _options.OpenAI.Temperature,
            MaxTokens = _options.OpenAI.MaxTokens
        };

        return await GenerateAnalysisAsync(request, cancellationToken);
    }

    public async Task<AiResponse> GenerateAnalysisAsync(AiRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        for (int providerIndex = 0; providerIndex < _providers.Count; providerIndex++)
        {
            var provider = _providers[providerIndex];
            var isFromFallback = providerIndex > 0;

            for (int attempt = 1; attempt <= _options.MaxRetries; attempt++)
            {
                try
                {
                    _logger.LogDebug("Attempting request with provider: {Provider} (attempt {Attempt}/{MaxRetries})",
                        provider.ProviderName, attempt, _options.MaxRetries);

                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    cts.CancelAfter(TimeSpan.FromSeconds(_options.TimeoutSeconds));

                    var content = await provider.GenerateResponseAsync(request.Prompt, cts.Token);

                    stopwatch.Stop();

                    var response = new AiResponse
                    {
                        Content = content,
                        Provider = provider.ProviderName,
                        ResponseTime = stopwatch.Elapsed,
                        IsFromFallback = isFromFallback
                    };

                    _logger.LogInformation("AI request completed successfully with provider: {Provider} in {ResponseTime}ms",
                        provider.ProviderName, stopwatch.ElapsedMilliseconds);

                    return response;
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("AI request was cancelled by caller");
                    throw;
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("AI request timed out with provider: {Provider} (attempt {Attempt})",
                        provider.ProviderName, attempt);
                }
                catch (AiException ex)
                {
                    _logger.LogWarning(ex, "AI provider {Provider} failed (attempt {Attempt}): {Message}",
                        provider.ProviderName, attempt, ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error with provider: {Provider} (attempt {Attempt})",
                        provider.ProviderName, attempt);
                }

                if (attempt < _options.MaxRetries)
                {
                    var delay = TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 1000); // Exponential backoff
                    _logger.LogDebug("Retrying in {Delay}ms", delay.TotalMilliseconds);
                    await Task.Delay(delay, cancellationToken);
                }
            }

            _logger.LogWarning("All retry attempts exhausted for provider: {Provider}", provider.ProviderName);
        }

        stopwatch.Stop();

        // If all providers failed, return a fallback response
        _logger.LogError("All AI providers failed, returning fallback response");

        return new AiResponse
        {
            Content = GenerateFallbackResponse(request.Prompt),
            Provider = "Fallback",
            ResponseTime = stopwatch.Elapsed,
            IsFromFallback = true
        };
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        if (_providers.Count == 0) return false;

        // Check if at least one provider is available
        var availabilityTasks = _providers.Select(p => p.IsAvailableAsync(cancellationToken));
        var results = await Task.WhenAll(availabilityTasks);

        return results.Any(r => r);
    }

    private static string GenerateFallbackResponse(string prompt)
    {
        return @"## System Notice: AI Service Temporarily Unavailable

I apologize, but our AI analysis service is currently experiencing technical difficulties. However, I can provide some general guidance:

### Immediate Actions:
1. **Document everything** - Preserve all relevant evidence and communications
2. **Timeline review** - Check all applicable statutes of limitations and deadlines
3. **Legal research** - Begin preliminary research on relevant statutes and case law
4. **Professional consultation** - Consider consulting with qualified legal counsel

### General Legal Strategy Framework:
- **Fact Development**: Gather and organize all relevant information
- **Legal Research**: Identify applicable laws and precedents
- **Risk Assessment**: Evaluate potential exposures and opportunities
- **Action Plan**: Develop step-by-step strategy with timelines

### Important Disclaimer:
This fallback response is for general informational purposes only and does not constitute legal advice. Please consult with a qualified attorney for specific legal guidance related to your matter.

---
*Please try again later when our AI services are restored for a comprehensive analysis.*";
    }
}
