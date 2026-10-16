using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcxiomCRM.Web.Data;
using AcxiomCRM.Web.Models;
using AcxiomCRM.Web.Services;
using Microsoft.AspNetCore.Identity;

namespace AcxiomCRM.Web.Controllers
{
    [Authorize]
    public class LeadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditService _auditService;
        private readonly UserManager<ApplicationUser> _userManager;

        public LeadController(ApplicationDbContext context, AuditService auditService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _auditService = auditService;
            _userManager = userManager;
        }

        // GET: Lead
        public async Task<IActionResult> Index(string searchString)
        {
            var leadsQuery = _context.Leads.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                leadsQuery = leadsQuery.Where(l => l.Title.Contains(searchString) || l.ContactName.Contains(searchString) || l.Email.Contains(searchString));
                ViewData["CurrentFilter"] = searchString;
            }

            return View(await leadsQuery.OrderByDescending(l => l.CreatedDate).ToListAsync());
        }

        // GET: Lead/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lead/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,ContactName,Email,Phone,Source,Status,EstimatedValue")] Lead lead)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User) ?? "System";
                lead.CreatedDate = DateTime.UtcNow;
                lead.AssignedToUserId = userId;

                _context.Add(lead);
                await _context.SaveChangesAsync();

                await _auditService.LogActionAsync(userId, "Create", "Lead", $"Created lead {lead.Title}");
                return RedirectToAction(nameof(Index));
            }
            return View(lead);
        }

        // GET: Lead/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var lead = await _context.Leads.FindAsync(id);
            if (lead == null) return NotFound();

            return View(lead);
        }

        // POST: Lead/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LeadId,Title,ContactName,Email,Phone,Source,Status,EstimatedValue,CreatedDate,AssignedToUserId")] Lead lead)
        {
            if (id != lead.LeadId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lead);
                    await _context.SaveChangesAsync();

                    var userId = _userManager.GetUserId(User) ?? "System";
                    await _auditService.LogActionAsync(userId, "Update", "Lead", $"Updated lead {lead.Title}");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Leads.Any(e => e.LeadId == lead.LeadId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(lead);
        }

        // GET: Lead/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var lead = await _context.Leads.FirstOrDefaultAsync(m => m.LeadId == id);
            if (lead == null) return NotFound();

            return View(lead);
        }

        // POST: Lead/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lead = await _context.Leads.FindAsync(id);
            if (lead != null)
            {
                _context.Leads.Remove(lead);
                await _context.SaveChangesAsync();

                var userId = _userManager.GetUserId(User) ?? "System";
                await _auditService.LogActionAsync(userId, "Delete", "Lead", $"Deleted lead ID {id}");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
