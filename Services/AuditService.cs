using AcxiomCRM.Web.Data;
using AcxiomCRM.Web.Models;

namespace AcxiomCRM.Web.Services
{
    public class AuditService
    {
        private readonly ApplicationDbContext _context;

        public AuditService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogActionAsync(string userId, string action, string entityName, string details)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityName = entityName,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
    }
}
