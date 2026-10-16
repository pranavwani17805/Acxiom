using AcxiomCRM.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Web.Services
{
    public class ReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalCustomersCountAsync()
        {
            return await _context.Customers.CountAsync();
        }

        public async Task<int> GetTotalLeadsCountAsync()
        {
            return await _context.Leads.CountAsync();
        }

        public async Task<decimal> GetTotalPipelineValueAsync()
        {
            // SQLite workaround: fetch decimal amounts and aggregate on client side
            var amounts = await _context.Opportunities.Select(o => o.Amount).ToListAsync();
            return amounts.Sum();
        }
    }
}
