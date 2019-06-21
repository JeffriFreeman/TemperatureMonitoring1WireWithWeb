using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MonitoringWeb
{
    public class SensorsTemp
    {
        public int Number { get; set; } //Number
        public string Id { get; set; }
        public string Mount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N1}")]
        public double Tempreture { get; set; }
        public bool CRC { get; set; }
        public DateTime LastGet { get; set; }
    }
}
