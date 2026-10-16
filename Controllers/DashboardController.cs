using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AcxiomCRM.Web.Services;

namespace AcxiomCRM.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ReportService _reportService;

        public DashboardController(ReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalCustomers = await _reportService.GetTotalCustomersCountAsync();
            ViewBag.TotalLeads = await _reportService.GetTotalLeadsCountAsync();
            ViewBag.PipelineValue = await _reportService.GetTotalPipelineValueAsync();

            return View();
        }
    }
}
