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
    public class ActivityController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditService _auditService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ActivityController(ApplicationDbContext context, AuditService auditService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _auditService = auditService;
            _userManager = userManager;
        }

        // GET: Activity
        public async Task<IActionResult> Index(string searchString)
        {
            var activitiesQuery = _context.Activities.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                activitiesQuery = activitiesQuery.Where(a => a.Subject.Contains(searchString) || a.Description.Contains(searchString) || a.ActivityType.Contains(searchString));
                ViewData["CurrentFilter"] = searchString;
            }

            return View(await activitiesQuery.OrderByDescending(a => a.DueDate).ToListAsync());
        }

        // GET: Activity/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Activity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Subject,ActivityType,Description,DueDate,IsCompleted")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User) ?? "System";
                activity.CreatedDate = DateTime.UtcNow;
                activity.UserId = userId;

                _context.Add(activity);
                await _context.SaveChangesAsync();

                await _auditService.LogActionAsync(userId, "Create", "Activity", $"Created activity {activity.Subject}");
                return RedirectToAction(nameof(Index));
            }
            return View(activity);
        }

        // GET: Activity/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var activity = await _context.Activities.FindAsync(id);
            if (activity == null) return NotFound();

            return View(activity);
        }

        // POST: Activity/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ActivityId,Subject,ActivityType,Description,DueDate,IsCompleted,CreatedDate,UserId")] Activity activity)
        {
            if (id != activity.ActivityId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activity);
                    await _context.SaveChangesAsync();

                    var userId = _userManager.GetUserId(User) ?? "System";
                    await _auditService.LogActionAsync(userId, "Update", "Activity", $"Updated activity {activity.Subject}");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Activities.Any(e => e.ActivityId == activity.ActivityId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(activity);
        }

        // GET: Activity/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var activity = await _context.Activities.FirstOrDefaultAsync(m => m.ActivityId == id);
            if (activity == null) return NotFound();

            return View(activity);
        }

        // POST: Activity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity != null)
            {
                _context.Activities.Remove(activity);
                await _context.SaveChangesAsync();

                var userId = _userManager.GetUserId(User) ?? "System";
                await _auditService.LogActionAsync(userId, "Delete", "Activity", $"Deleted activity ID {id}");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
