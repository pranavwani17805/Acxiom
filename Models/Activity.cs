using System;

namespace AcxiomCRM.Web.Models
{
    public class Activity
    {
        public int ActivityId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty; // Call, Meeting, Email, Task
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(1);
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = string.Empty;
    }
}
