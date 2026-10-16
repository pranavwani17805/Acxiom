using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcxiomCRM.Web.Data;

namespace AcxiomCRM.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var auditQuery = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                auditQuery = auditQuery.Where(a => a.Action.Contains(searchString) 
                    || a.EntityName.Contains(searchString) 
                    || a.Details.Contains(searchString) 
                    || a.UserId.Contains(searchString));
                ViewData["CurrentFilter"] = searchString;
            }

            return View(await auditQuery.OrderByDescending(a => a.Timestamp).Take(100).ToListAsync());
        }
    }
}
