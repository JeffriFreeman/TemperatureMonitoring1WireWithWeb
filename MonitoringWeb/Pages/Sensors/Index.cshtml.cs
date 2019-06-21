using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MonitoringWeb;
using MonitoringWeb.Models;

namespace MonitoringWeb.Pages.Sensors
{
    public class IndexModel : PageModel
    {
        private readonly MonitoringWeb.Models.MonitoringWebContext _context;

        public IndexModel(MonitoringWeb.Models.MonitoringWebContext context)
        {
            _context = context;
        }

        public IList<SensorsTemp> SensorsTemp { get; set; }
        public string Mount { get; private set; }
        public SelectList Mounts { get; private set; }

        public async Task OnGetAsync(string searchString, string mount)
        {
            {
                IQueryable<string> mountIndex = from m in _context.Sensors
                                                orderby m.Mount
                                                select m.Mount;



                var sensorsTemp = from m in _context.Sensors
                                  select m;

                foreach (var e in sensorsTemp) e.Number++; //получим значение реального номера, а не в массиве (первым идет не 0, а 1)

                if (!String.IsNullOrEmpty(searchString))
                {
                    sensorsTemp = _context.Sensors
                        .Where(context => context.Tempreture > 0 && context.Number == int.Parse(searchString))
                        .OrderByDescending(context => context.Number);
                }
                else if (!String.IsNullOrEmpty(mount))
                {
                    sensorsTemp = _context.Sensors
                        .Where(context => context.Tempreture > 0 && context.Mount == mount)
                        .OrderByDescending(context => context.Number);
                }
                else
                {
                    sensorsTemp = _context.Sensors
                        .Where(context => context.Tempreture > 0)
                        .OrderByDescending(context => context.Number);
                }
                Mounts = new SelectList(await mountIndex.Distinct().ToListAsync());
                SensorsTemp = await sensorsTemp.ToListAsync();
            }
        }
    }
}
