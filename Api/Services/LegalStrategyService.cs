using LegalStrategyAdvisor.Api.Services.AI;

namespace LegalStrategyAdvisor.Api.Services
{
    public interface ILegalStrategyService
    {
        Task<string> GenerateStrategyAsync(string caseDescription, CancellationToken cancellationToken = default);
        Task<bool> IsAiAvailableAsync(CancellationToken cancellationToken = default);
    }

    public class LegalStrategyService : ILegalStrategyService
    {
        private readonly IAiService _aiService;
        private readonly ILogger<LegalStrategyService> _logger;

        public LegalStrategyService(IAiService aiService, ILogger<LegalStrategyService> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        public async Task<string> GenerateStrategyAsync(string caseDescription, CancellationToken cancellationToken = default)
        {
            try
            {
                // Input validation
                if (string.IsNullOrWhiteSpace(caseDescription))
                {
                    throw new ArgumentException("Case description cannot be empty", nameof(caseDescription));
                }

                if (caseDescription.Length > 10000)
                {
                    throw new ArgumentException("Case description too long (max 10,000 characters)", nameof(caseDescription));
                }

                _logger.LogInformation("Generating legal strategy for case description of length: {Length}", caseDescription.Length);

                var response = await _aiService.GenerateStrategyAsync(caseDescription, cancellationToken);

                _logger.LogInformation("Legal strategy generated successfully using provider: {Provider} (from fallback: {IsFromFallback})",
                    response.Provider, response.IsFromFallback);

                return response.Content;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Legal strategy generation was cancelled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating legal strategy");
                throw;
            }
        }

        public async Task<bool> IsAiAvailableAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _aiService.IsAvailableAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking AI availability");
                return false;
            }
        }
    }
}
