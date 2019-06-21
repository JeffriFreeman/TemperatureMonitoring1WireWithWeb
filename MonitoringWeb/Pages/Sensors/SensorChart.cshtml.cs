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
    public class SensorChartModel : PageModel
    {
        private readonly MonitoringWeb.Models.MonitoringWebContext _context;

        public SensorChartModel(MonitoringWeb.Models.MonitoringWebContext context)
        {
            _context = context;
        }

        public IList<SensorsTemp> SensorsTemp { get;set; }
        public IList<SensorsTempData> SensorsTempData { get; set; }
        public string XLabels { get; set; }
        public string YValues { get; set; }
        public string LastId { get; set; }
        public DateTime DateMax { get; set; }
        public DateTime DateMin { get; set; }

        public async Task OnGetAsync(string id, string dateStringMin, string dateStringMax)
        {
            //SensorsTemp = await _context.Sensors.ToListAsync();
            SensorsTempData = await _context.SensorsData.ToListAsync();
            LastId = id;
            
            if (dateStringMin == null || dateStringMax == null)
            {
                DateMin = DateTime.Now.AddHours(-1);
                DateMax = DateTime.Now.AddHours(0);
            }
            else
            {
                DateMin = DateTime.Parse(dateStringMin);
                DateMax = DateTime.Parse(dateStringMax);
            }

            XLabels = Newtonsoft.Json.JsonConvert.SerializeObject(
                SensorsTempData
                .Where(z => z.Tempreture > 0 && z.Id == id && z.LastGet > DateMin && z.LastGet < DateMax)
                .OrderBy(z => z.Number)
                .Select(x => x.LastGet)
                .ToList());
            YValues = Newtonsoft.Json.JsonConvert.SerializeObject(
                SensorsTempData
                .Where(z => z.Tempreture > 0 && z.Id == id && z.LastGet > DateMin && z.LastGet < DateMax)
                .OrderBy(z => z.Number)
                .Select(x => x.Tempreture)
                .ToList());
        }
    }
}
