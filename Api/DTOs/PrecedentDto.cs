using System.ComponentModel.DataAnnotations;

namespace LegalStrategyAdvisor.Api.DTOs
{
    public record PrecedentDto(
        int Id,
        string Citation,
        string Summary,
        string Jurisdiction,
        int Year,
        string CaseType,
        double RelevanceScore
    );

    public record PrecedentSearchRequest(
        [StringLength(100)] string? Jurisdiction = null,
        [StringLength(50)] string? CaseType = null,
        [Range(1000, 2100)] int? Year = null,
        [StringLength(200)] string? SearchTerm = null,
        [Range(1, 100)] int PageSize = 10,
        [Range(1, int.MaxValue)] int Page = 1
    );

    public record PrecedentSearchResponse(
        IReadOnlyList<PrecedentDto> Precedents,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages
    );
}
