using System.Text.Json;

namespace LegalStrategyAdvisor.Api.Services.AI;

public interface IPythonAiClient
{
    Task<PythonStrategyResponse> GenerateStrategyAsync(string caseDescription, CancellationToken cancellationToken = default);
    Task<PythonHealthResponse> GetHealthAsync(CancellationToken cancellationToken = default);
}

public class PythonAiClient : IPythonAiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PythonAiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public PythonAiClient(HttpClient httpClient, ILogger<PythonAiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<PythonStrategyResponse> GenerateStrategyAsync(string caseDescription, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new PythonStrategyRequest { CaseDescription = caseDescription };
            var requestJson = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");

            _logger.LogDebug("Sending strategy request to Python AI service");

            var response = await _httpClient.PostAsync("/api/strategy", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<PythonStrategyResponse>(responseJson, _jsonOptions);

            _logger.LogInformation("Strategy received from Python AI service using provider: {Provider}", result?.Provider);

            return result ?? throw new InvalidOperationException("Received null response from Python AI service");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error when calling Python AI service");
            throw new AiException("PythonAI", $"Failed to connect to Python AI service: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (ex.CancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Python AI service request was cancelled");
            throw;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Python AI service request timed out");
            throw new AiException("PythonAI", "Request to Python AI service timed out", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response from Python AI service");
            throw new AiException("PythonAI", "Invalid response format from Python AI service", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling Python AI service");
            throw new AiException("PythonAI", $"Unexpected error: {ex.Message}", ex);
        }
    }

    public async Task<PythonHealthResponse> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking Python AI service health");

            var response = await _httpClient.GetAsync("/api/health", cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<PythonHealthResponse>(responseJson, _jsonOptions);

            return result ?? throw new InvalidOperationException("Received null health response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking Python AI service health");
            return new PythonHealthResponse
            {
                Status = "error",
                Provider = "unknown",
                ProviderAvailable = false
            };
        }
    }
}

// DTOs for Python AI service communication
public class PythonStrategyRequest
{
    public string CaseDescription { get; set; } = string.Empty;
}

public class PythonStrategyResponse
{
    public string Strategy { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public int? TokensUsed { get; set; }
    public double? ProcessingTime { get; set; }
}

public class PythonHealthResponse
{
    public string Status { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public bool ProviderAvailable { get; set; }
}
