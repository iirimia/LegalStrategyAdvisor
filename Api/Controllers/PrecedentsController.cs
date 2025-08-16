using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LegalStrategyAdvisor.Api.Data;
using LegalStrategyAdvisor.Api.DTOs;
using LegalStrategyAdvisor.Api.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace LegalStrategyAdvisor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrecedentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PrecedentsController> _logger;

        public PrecedentsController(AppDbContext context, ILogger<PrecedentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Search precedents with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PrecedentSearchResponse>> SearchPrecedents(
            [FromQuery] PrecedentSearchRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Input validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var query = _context.Precedents.AsQueryable();

                // Apply filters with input sanitization
                if (!string.IsNullOrWhiteSpace(request.Jurisdiction))
                {
                    var jurisdiction = request.Jurisdiction.Trim().Substring(0, Math.Min(100, request.Jurisdiction.Length));
                    query = query.Where(p => p.Jurisdiction.Contains(jurisdiction));
                }

                if (!string.IsNullOrWhiteSpace(request.CaseType))
                {
                    var caseType = request.CaseType.Trim().Substring(0, Math.Min(50, request.CaseType.Length));
                    query = query.Where(p => p.CaseType.Contains(caseType));
                }

                if (request.Year.HasValue)
                {
                    query = query.Where(p => p.Year == request.Year.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.Trim().Substring(0, Math.Min(200, request.SearchTerm.Length));
                    query = query.Where(p =>
                        p.Citation.Contains(searchTerm) ||
                        p.Summary.Contains(searchTerm));
                }

                // Get total count before pagination
                var totalCount = await query.CountAsync(cancellationToken);

                // Apply pagination
                var precedents = await query
                    .OrderByDescending(p => p.RelevanceScore)
                    .ThenByDescending(p => p.Year)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(p => new PrecedentDto(
                        p.Id,
                        p.Citation,
                        p.Summary,
                        p.Jurisdiction,
                        p.Year,
                        p.CaseType,
                        p.RelevanceScore
                    ))
                    .ToListAsync(cancellationToken);

                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                var response = new PrecedentSearchResponse(
                    precedents,
                    totalCount,
                    request.Page,
                    request.PageSize,
                    totalPages
                );

                _logger.LogInformation("Precedent search completed. Found {Count} results", totalCount);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching precedents");
                return Problem("An error occurred while searching precedents");
            }
        }

        /// <summary>
        /// Get a specific precedent by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PrecedentDto>> GetPrecedent(
            [Range(1, int.MaxValue)] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var precedent = await _context.Precedents
                    .Where(p => p.Id == id)
                    .Select(p => new PrecedentDto(
                        p.Id,
                        p.Citation,
                        p.Summary,
                        p.Jurisdiction,
                        p.Year,
                        p.CaseType,
                        p.RelevanceScore
                    ))
                    .FirstOrDefaultAsync(cancellationToken);

                if (precedent == null)
                {
                    return NotFound($"Precedent with ID {id} not found");
                }

                return Ok(precedent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving precedent {Id}", id);
                return Problem($"An error occurred while retrieving precedent {id}");
            }
        }

        /// <summary>
        /// Get available case types for filtering
        /// </summary>
        [HttpGet("case-types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCaseTypes(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var caseTypes = await _context.Precedents
                    .Where(p => !string.IsNullOrEmpty(p.CaseType))
                    .Select(p => p.CaseType)
                    .Distinct()
                    .OrderBy(ct => ct)
                    .ToListAsync(cancellationToken);

                return Ok(caseTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving case types");
                return Problem("An error occurred while retrieving case types");
            }
        }

        /// <summary>
        /// Get available jurisdictions for filtering
        /// </summary>
        [HttpGet("jurisdictions")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetJurisdictions(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var jurisdictions = await _context.Precedents
                    .Where(p => !string.IsNullOrEmpty(p.Jurisdiction))
                    .Select(p => p.Jurisdiction)
                    .Distinct()
                    .OrderBy(j => j)
                    .ToListAsync(cancellationToken);

                return Ok(jurisdictions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving jurisdictions");
                return Problem("An error occurred while retrieving jurisdictions");
            }
        }
    }
}
