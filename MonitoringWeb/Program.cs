using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MonitoringWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var GroupSensors = new GroupOfSensorsTemp();
            var thread = new Thread(() => GroupSensors.RunSSH());
            thread.Start();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        
    }
}
