using System.ComponentModel.DataAnnotations;

namespace LegalStrategyAdvisor.Api.Data.Entities
{
    public class Case
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
