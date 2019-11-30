using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Linq;

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

            modelBuilder.Entity<Log>()
                .Property(h => h.Id)
                .ValueGeneratedOnAdd();

            Random rnd = new Random();
            var ips = Enumerable.Range(0, 10000).Select(i => {
                int i1 = rnd.Next(1,256);
                int i2 = rnd.Next(1,256);
                int i3 = rnd.Next(1,256);
                int i4 = rnd.Next(1,256);
                return $"{i1}.{i2}.{i3}.{i4}";
            }).Distinct( )
            .Where( s=> string.IsNullOrWhiteSpace(s)==false)
            .Select( s => new Host { IpAddress = s, Condition = true, Name = s })
            .ToArray();

            modelBuilder.Entity<Host>().HasData(
                ips
            /*    new Host {  Name = "Google DNS", Condition = true, IpAddress = "8.8.8.8" },
                new Host { Name = "Quadro One", Condition = true, IpAddress = "1.1.1.1" },
                new Host { Name = "Untitled", Condition = false, IpAddress = "14.15.22.9" },
                new Host {  Name = "Are u exist?", Condition = true, IpAddress = "111.111.111.111" }
              */ );

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