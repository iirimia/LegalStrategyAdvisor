using System.ComponentModel.DataAnnotations;

namespace LegalStrategyAdvisor.Api.Data.Entities
{
    public class LegalStrategy
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
