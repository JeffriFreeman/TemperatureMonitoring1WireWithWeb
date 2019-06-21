using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MonitoringWeb;
using MonitoringWeb.Models;

namespace MonitoringWeb.Pages.Sensors
{
    public class DetailsModel : PageModel
    {
        private readonly MonitoringWeb.Models.MonitoringWebContext _context;

        public DetailsModel(MonitoringWeb.Models.MonitoringWebContext context)
        {
            _context = context;
        }

        public SensorsTemp SensorsTemp { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SensorsTemp = await _context.Sensors.FirstOrDefaultAsync(m => m.Id == id);

            if (SensorsTemp == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
