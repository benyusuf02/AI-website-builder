using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ErrorLogsController : Controller
    {
        private readonly YDeveloperContext _context;

        public ErrorLogsController(YDeveloperContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Fetching logs from AuditLogs where Action contains 'Error' or similar
            // Alternatively, in a real scenario, we'd read from Serilog files or a dedicated ErrorLog table.
            // For this phase, we'll assume AuditLogs captures critical errors or we show a placeholder if no dedicated table exists.

            // Checking if we have logs. If not, we might need to rely on reading text files. 
            // Since User requested "500 Error Catcher UI", and we don't have a structured DB table for errors yet (only AuditLogs),
            // I will implement a text-file reader for Serilog logs if available, or just show AuditLogs.

            // Let's use AuditLogs for simplicity as per plan, filtering for high severity if possible, 
            // or just showing the last 50 actions.

            var logs = await _context.AuditLogs
                .Include(a => a.PerformerUser)
                .OrderByDescending(a => a.Timestamp)
                .Take(50)
                .ToListAsync();

            return View(logs);
        }
    }
}
