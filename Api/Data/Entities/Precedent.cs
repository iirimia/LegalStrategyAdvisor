using System.ComponentModel.DataAnnotations;

namespace LegalStrategyAdvisor.Api.Data.Entities
{
    public class Precedent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Citation { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Summary { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Jurisdiction { get; set; } = string.Empty;

        public int Year { get; set; }

        [StringLength(50)]
        public string CaseType { get; set; } = string.Empty;

        [Range(0, 1)]
        public double RelevanceScore { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
