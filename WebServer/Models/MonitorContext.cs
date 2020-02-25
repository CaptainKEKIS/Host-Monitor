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
            var ips = Enumerable.Range(0, 20).Select(i => {
                int i1 = rnd.Next(1,256);
                int i2 = rnd.Next(1,256);
                int i3 = rnd.Next(1,256);
                int i4 = rnd.Next(1,256);
                return $"{i1}.{i2}.{i3}.{i4}";
            }).Distinct( )
            .Where( s=> string.IsNullOrWhiteSpace(s)==false)
            .Select( s => new Host { IpAddress = s, Condition = true, Name = s })
            .ToArray();

            modelBuilder.Entity<Host>().HasData(ips);
        }
    }
}