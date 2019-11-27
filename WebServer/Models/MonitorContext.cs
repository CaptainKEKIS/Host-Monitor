using Microsoft.EntityFrameworkCore;
using System;

namespace WebServer.Models
{
    public class MonitorContext : DbContext
    {
        public DbSet<Host> Hosts { get; set; }
        public DbSet<Log> Logs { get; set; }
        public MonitorContext(DbContextOptions<MonitorContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //int id = 1;
            modelBuilder.Entity<Host>()
                .Property(h => h.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Log>()
                .Property(h => h.Id)
                .ValueGeneratedOnAdd();


            modelBuilder.Entity<Host>().HasData(
                new Host { Id = 1, Name = "Google DNS", Condition = true, IpAddress = "8.8.8.8" },
                new Host { Id = 2, Name = "Quadro One", Condition = true, IpAddress = "1.1.1.1" },
                new Host { Id = 3,  Name = "Untitled", Condition = false, IpAddress = "14.15.22.9" },
                new Host { Id = 4,  Name = "Are u exist?", Condition = true, IpAddress = "111.111.111.111" }
               );

            //modelBuilder.Entity<Log>().HasData(
            //    new Log { Delay = 5, HostId = 1, Status = "OK!", TimeStamp = DateTime.Now },
            //    new Log { Delay = 1, HostId = 1, Status = "OK!", TimeStamp = DateTime.Now },
            //    new Log { Delay = 6, HostId = 1, Status = "OK!", TimeStamp = DateTime.Now },
            //    new Log { Delay = 2, HostId = 1, Status = "OK!", TimeStamp = DateTime.Now },
            //    new Log { Delay = 3, HostId = 1, Status = "OK!", TimeStamp = DateTime.Now },
            //    new Log { Delay = 5, HostId = 1, Status = "OK!", TimeStamp = DateTime.Now }
            //    );
        }
    }
}