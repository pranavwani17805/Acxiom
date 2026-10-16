using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Web.Models
{
    public class Opportunity
    {
        [Key]
        public int OpportunityId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public decimal Amount { get; set; }
        public string Stage { get; set; } = "Prospecting";
        public DateTime ClosingDate { get; set; }
        public decimal Probability { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string AssignedToUserId { get; set; } = string.Empty;
    }
}
