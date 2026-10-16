using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Web.Models
{
    public class Lead
    {
        [Key]
        public int LeadId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string ContactName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Status { get; set; } = "New";
        public decimal EstimatedValue { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string AssignedToUserId { get; set; } = string.Empty;
    }
}
