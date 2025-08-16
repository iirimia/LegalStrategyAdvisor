using Microsoft.AspNetCore.Mvc;
using LegalStrategyAdvisor.Api.Services;
using LegalStrategyAdvisor.Api.Services.AI;
using System.ComponentModel.DataAnnotations;

namespace LegalStrategyAdvisor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StrategyController : ControllerBase
{
    private readonly ILegalStrategyService _legalStrategyService;
    private readonly IAiService _aiService;
    private readonly ILogger<StrategyController> _logger;

    public StrategyController(
        ILegalStrategyService legalStrategyService,
        IAiService aiService,
        ILogger<StrategyController> logger)
    {
        _legalStrategyService = legalStrategyService;
        _aiService = aiService;
        _logger = logger;
    }

    /// <summary>
    /// Generates a comprehensive legal strategy based on case description
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<StrategyResponse>> GenerateStrategy(
        [FromBody] StrategyRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.CaseDescription))
            {
                return BadRequest(new { error = "Case description is required" });
            }

            if (request.CaseDescription.Length > 10000)
            {
                return BadRequest(new { error = "Case description too long (max 10,000 characters)" });
            }

            _logger.LogInformation("Generating strategy for case of length: {Length}", request.CaseDescription.Length);

            var strategy = await _legalStrategyService.GenerateStrategyAsync(request.CaseDescription, cancellationToken);

            return Ok(new StrategyResponse
            {
                Strategy = strategy,
                GeneratedAt = DateTime.UtcNow,
                CaseDescription = request.CaseDescription
            });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(408, new { error = "Request timeout" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating strategy");
            return StatusCode(500, new { error = "An error occurred while generating the strategy" });
        }
    }

    /// <summary>
    /// Generates custom AI analysis based on detailed request
    /// </summary>
    [HttpPost("analyze")]
    public async Task<ActionResult<AiAnalysisResponse>> GenerateAnalysis(
        [FromBody] AiAnalysisRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest(new { error = "Prompt is required" });
            }

            if (request.Prompt.Length > 15000)
            {
                return BadRequest(new { error = "Prompt too long (max 15,000 characters)" });
            }

            _logger.LogInformation("Generating AI analysis for prompt of length: {Length}", request.Prompt.Length);

            var aiRequest = new AiRequest
            {
                Prompt = request.Prompt,
                SystemPrompt = request.SystemPrompt ?? string.Empty,
                Temperature = request.Temperature ?? 0.7,
                MaxTokens = request.MaxTokens ?? 2000
            };

            var response = await _aiService.GenerateAnalysisAsync(aiRequest, cancellationToken);

            return Ok(new AiAnalysisResponse
            {
                Content = response.Content,
                Provider = response.Provider,
                ResponseTime = response.ResponseTime,
                IsFromFallback = response.IsFromFallback,
                GeneratedAt = DateTime.UtcNow
            });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(408, new { error = "Request timeout" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating AI analysis");
            return StatusCode(500, new { error = "An error occurred while generating the analysis" });
        }
    }

    /// <summary>
    /// Checks AI service availability
    /// </summary>
    [HttpGet("status")]
    public async Task<ActionResult<AiStatusResponse>> GetAiStatus(CancellationToken cancellationToken = default)
    {
        try
        {
            var isAvailable = await _legalStrategyService.IsAiAvailableAsync(cancellationToken);

            return Ok(new AiStatusResponse
            {
                IsAvailable = isAvailable,
                CheckedAt = DateTime.UtcNow,
                Status = isAvailable ? "Available" : "Unavailable"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking AI status");
            return Ok(new AiStatusResponse
            {
                IsAvailable = false,
                CheckedAt = DateTime.UtcNow,
                Status = "Error",
                ErrorMessage = "Unable to check AI service status"
            });
        }
    }
}

// DTOs
public class StrategyRequest
{
    [Required]
    [StringLength(10000, MinimumLength = 10)]
    public string CaseDescription { get; set; } = string.Empty;
}

public class StrategyResponse
{
    public string Strategy { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string CaseDescription { get; set; } = string.Empty;
}

public class AiAnalysisRequest
{
    [Required]
    [StringLength(15000, MinimumLength = 5)]
    public string Prompt { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? SystemPrompt { get; set; }

    [Range(0.0, 2.0)]
    public double? Temperature { get; set; }

    [Range(100, 4000)]
    public int? MaxTokens { get; set; }
}

public class AiAnalysisResponse
{
    public string Content { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public TimeSpan ResponseTime { get; set; }
    public bool IsFromFallback { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class AiStatusResponse
{
    public bool IsAvailable { get; set; }
    public DateTime CheckedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}
