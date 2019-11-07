using Microsoft.EntityFrameworkCore;
using System;

namespace WebServer.Models
{
    public class MonitorContext : DbContext
    {
        public DbSet<Host> Hosts { get; set; }
        public DbSet<Logs> Logs { get; set; }
        public MonitorContext(DbContextOptions<MonitorContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //int id = 1;
            modelBuilder.Entity<Host>().HasData(
                new Host { Id = 1, Name = "Google DNS", Condition = true, IpAddress = "8.8.8.8" },
                new Host { Id = 2, Name = "Quadro One", Condition = true, IpAddress = "1.1.1.1" },
                new Host { Id = 3, Name = "Untitled", Condition = false, IpAddress = "14.15.22.9" },
                new Host { Id = 4, Name = "Are u exist?", Condition = true, IpAddress = "111.111.111.111" }
               );

            modelBuilder.Entity<Logs>().HasData(
                new Logs { Id = 1, Delay = 5, HostId = 1, Status = "OK!", TimeStamp = DateTime.Now },
                new Logs { Id = 2, Delay = 1, HostId = 1, Status = "OK!", TimeStamp = DateTime.Now },
                new Logs { Id = 3, Delay = 6, HostId = 1, Status = "OK!", TimeStamp = DateTime.Now },
                new Logs { Id = 4, Delay = 2, HostId = 1, Status = "OK!", TimeStamp = DateTime.Now },
                new Logs { Id = 5, Delay = 3, HostId = 1, Status = "OK!", TimeStamp = DateTime.Now },
                new Logs { Id = 6, Delay = 5, HostId = 1, Status = "OK!", TimeStamp = DateTime.Now }
                );
        }
    }
}