using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AcxiomCRM.Web.Data;
using AcxiomCRM.Web.Models;
using AcxiomCRM.Web.Services;
using Microsoft.AspNetCore.Identity;

namespace AcxiomCRM.Web.Controllers
{
    [Authorize]
    public class OpportunityController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditService _auditService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OpportunityController(ApplicationDbContext context, AuditService auditService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _auditService = auditService;
            _userManager = userManager;
        }

        // GET: Opportunity
        public async Task<IActionResult> Index(string searchString)
        {
            var opportunitiesQuery = _context.Opportunities.Include(o => o.Customer).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                opportunitiesQuery = opportunitiesQuery.Where(o => o.Name.Contains(searchString) || (o.Customer != null && o.Customer.CustomerName.Contains(searchString)));
                ViewData["CurrentFilter"] = searchString;
            }

            return View(await opportunitiesQuery.OrderByDescending(o => o.CreatedDate).ToListAsync());
        }

        // GET: Opportunity/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.CustomerId = new SelectList(await _context.Customers.OrderBy(c => c.CustomerName).ToListAsync(), "CustomerId", "CustomerName");
            return View();
        }

        // POST: Opportunity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CustomerId,Amount,Stage,ClosingDate,Probability")] Opportunity opportunity)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User) ?? "System";
                opportunity.CreatedDate = DateTime.UtcNow;
                opportunity.AssignedToUserId = userId;

                _context.Add(opportunity);
                await _context.SaveChangesAsync();

                await _auditService.LogActionAsync(userId, "Create", "Opportunity", $"Created opportunity {opportunity.Name}");
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CustomerId = new SelectList(await _context.Customers.OrderBy(c => c.CustomerName).ToListAsync(), "CustomerId", "CustomerName", opportunity.CustomerId);
            return View(opportunity);
        }

        // GET: Opportunity/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var opportunity = await _context.Opportunities.FindAsync(id);
            if (opportunity == null) return NotFound();

            ViewBag.CustomerId = new SelectList(await _context.Customers.OrderBy(c => c.CustomerName).ToListAsync(), "CustomerId", "CustomerName", opportunity.CustomerId);
            return View(opportunity);
        }

        // POST: Opportunity/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OpportunityId,Name,CustomerId,Amount,Stage,ClosingDate,Probability,CreatedDate,AssignedToUserId")] Opportunity opportunity)
        {
            if (id != opportunity.OpportunityId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(opportunity);
                    await _context.SaveChangesAsync();

                    var userId = _userManager.GetUserId(User) ?? "System";
                    await _auditService.LogActionAsync(userId, "Update", "Opportunity", $"Updated opportunity {opportunity.Name}");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Opportunities.Any(e => e.OpportunityId == opportunity.OpportunityId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CustomerId = new SelectList(await _context.Customers.OrderBy(c => c.CustomerName).ToListAsync(), "CustomerId", "CustomerName", opportunity.CustomerId);
            return View(opportunity);
        }

        // GET: Opportunity/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var opportunity = await _context.Opportunities.Include(o => o.Customer).FirstOrDefaultAsync(m => m.OpportunityId == id);
            if (opportunity == null) return NotFound();

            return View(opportunity);
        }

        // POST: Opportunity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opportunity = await _context.Opportunities.FindAsync(id);
            if (opportunity != null)
            {
                _context.Opportunities.Remove(opportunity);
                await _context.SaveChangesAsync();

                var userId = _userManager.GetUserId(User) ?? "System";
                await _auditService.LogActionAsync(userId, "Delete", "Opportunity", $"Deleted opportunity ID {id}");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
