using System;
using System.Collections.Generic;
using System.Text;

namespace MonitoringWeb
{
    public class SensorsTempData
    {
        public string Id { get; set; }
        public string Mount { get; set; }

        public int Number { get; set; } //Number
        public double Tempreture { get; set; }
        public bool CRC { get; set; }
        public DateTime LastGet { get; set; }
    }
}
