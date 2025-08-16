using Microsoft.AspNetCore.Mvc;
using LegalStrategyAdvisor.Api.Services;
using LegalStrategyAdvisor.Api.Services.AI;

namespace LegalStrategyAdvisor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IAiService _aiService;

        public HealthController(IAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            try
            {
                var isAiAvailable = await _aiService.IsAvailableAsync(cancellationToken);
                return Ok(new
                {
                    status = "healthy",
                    aiAvailable = isAiAvailable,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception)
            {
                return Ok(new
                {
                    status = "healthy",
                    aiAvailable = false,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
