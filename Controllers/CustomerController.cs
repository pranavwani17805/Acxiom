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
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditService _auditService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(ApplicationDbContext context, AuditService auditService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _auditService = auditService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var customersQuery = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                customersQuery = customersQuery.Where(c => c.CustomerName.Contains(searchString) || c.Email.Contains(searchString) || c.CompanyName.Contains(searchString));
                ViewData["CurrentFilter"] = searchString;
            }

            return View(await customersQuery.OrderByDescending(c => c.CreatedDate).ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerCode,CustomerName,Email,Phone,CompanyName,Address,City,State,Status")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User) ?? "System";
                customer.CreatedDate = DateTime.UtcNow;
                customer.CreatedBy = userId;

                _context.Add(customer);
                await _context.SaveChangesAsync();

                await _auditService.LogActionAsync(userId, "Create", "Customer", $"Created customer {customer.CustomerName}");
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,CustomerCode,CustomerName,Email,Phone,CompanyName,Address,City,State,Status,CreatedDate,CreatedBy")] Customer customer)
        {
            if (id != customer.CustomerId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();

                    var userId = _userManager.GetUserId(User) ?? "System";
                    await _auditService.LogActionAsync(userId, "Update", "Customer", $"Updated customer {customer.CustomerName}");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Customers.Any(e => e.CustomerId == customer.CustomerId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                var userId = _userManager.GetUserId(User) ?? "System";
                await _auditService.LogActionAsync(userId, "Delete", "Customer", $"Deleted customer ID {id}");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
