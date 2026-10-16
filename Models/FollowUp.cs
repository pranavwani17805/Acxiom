using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Web.Models
{
    public class FollowUp
    {
        [Key]
        public int FollowUpId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string Priority { get; set; } = "Medium";
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string AssignedToUserId { get; set; } = string.Empty;
    }
}
