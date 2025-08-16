using LegalStrategyAdvisor.Api.Services.AI;

namespace LegalStrategyAdvisor.Api.Services.AI;

public class PythonAiProvider : IAiProvider
{
    private readonly IPythonAiClient _pythonClient;
    private readonly ILogger<PythonAiProvider> _logger;

    public string ProviderName => "PythonAI";

    public PythonAiProvider(IPythonAiClient pythonClient, ILogger<PythonAiProvider> logger)
    {
        _pythonClient = pythonClient;
        _logger = logger;
    }

    public async Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Sending request to Python AI service");

            var response = await _pythonClient.GenerateStrategyAsync(prompt, cancellationToken);

            _logger.LogInformation("Python AI service responded using provider: {Provider} in {ProcessingTime:F2}s",
                response.Provider, response.ProcessingTime ?? 0);

            return response.Strategy;
        }
        catch (AiException)
        {
            throw; // Re-throw AI exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Python AI service");
            throw new AiException(ProviderName, $"Python AI service error: {ex.Message}", ex);
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var health = await _pythonClient.GetHealthAsync(cancellationToken);
            return health.Status.Equals("healthy", StringComparison.OrdinalIgnoreCase) && health.ProviderAvailable;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Python AI service availability check failed");
            return false;
        }
    }
}
