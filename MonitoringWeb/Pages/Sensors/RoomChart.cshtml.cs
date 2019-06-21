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
    public class RoomChartModel : PageModel
    {
        private readonly MonitoringWeb.Models.MonitoringWebContext _context;

        public RoomChartModel(MonitoringWeb.Models.MonitoringWebContext context)
        {
            _context = context;
        }

        public IList<SensorsTemp> SensorsTemp { get;set; }
        public string XLabels { get; set; }
        public string YValues { get; set; }

        public async Task OnGetAsync()
        {
            SensorsTemp = await _context.Sensors.ToListAsync();
            foreach (var e in SensorsTemp) e.Number++; //получим значение реального номера, а не в массиве
            XLabels = Newtonsoft.Json.JsonConvert.SerializeObject(SensorsTemp.Where(z => z.Tempreture>0).OrderBy(z => z.Number).Select(x => x.Number).ToList());
            YValues = Newtonsoft.Json.JsonConvert.SerializeObject(SensorsTemp.Where(z => z.Tempreture > 0).OrderBy(z => z.Number).Select(x => x.Tempreture).ToList());
        }
    }
}
