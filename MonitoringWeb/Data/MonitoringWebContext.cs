using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MonitoringWeb;

namespace MonitoringWeb.Models
{
    public class MonitoringWebContext : DbContext
    {
        public MonitoringWebContext(DbContextOptions<MonitoringWebContext> options)
            : base(options)
        {
        }
        public MonitoringWebContext()
        {
        }

        public DbSet<MonitoringWeb.SensorsTemp> Sensors { get; set; }

        public DbSet<MonitoringWeb.SensorsTempData> SensorsData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=MonitoringWebContext.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SensorsTemp>()
                .HasKey(c => new { c.Id });
            modelBuilder.Entity<SensorsTempData>()
                .HasKey(c => new { c.Id, c.LastGet });
        }
    }
}
