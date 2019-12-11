using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using HMLib;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using WebServer.Models;

namespace WebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //MonitorContext _context;
            Settings settings = null;

            settings = ConfigHelper.GetSettings();       //получаю настройки из конфигурации
            string connection = ConfigHelper.GetDefaultConnection();        //получаю строку подключения из конфигурации
            var dbContextOptions = new DbContextOptionsBuilder<MonitorContext>()
                .UseSqlite(connection).Options;
            var _context = new MonitorContext(dbContextOptions);
            UpkServices.ServiceProvider.RegisterService(typeof(MonitorContext), typeof(MonitorContext), dbContextOptions);
            //UpkServices.ServiceProvider.RegisterService< MonitorContext>(_context);
            var context = UpkServices.ServiceProvider.GetService<MonitorContext>();
            //var address  = context.Hosts.First().IpAddress;

            //for (int i = 0; i < 10000; i++) {

            //    Parallel.For(0, 4, l=> {

            //        var logs = Enumerable.Range(0, 2500).Select(j => new Log { Delay = 1, Generation = 1, IpAddress = address, TimeStamp = DateTime.Now });
            //        ResultToDataBase(null, new PingerEventArgs() { PingResults = logs });
            //    });
            //    //context.AddRange(logs);
            //    Console.WriteLine(i);
            //    //Thread.Sleep(100);
            //}


            List<Models.Host> hosts = _context.Hosts.ToList();
            HostMonitor hostMonitor = new HostMonitor(settings, hosts);
            hostMonitor.OnPingCompleted += ResultToDataBase;
            hostMonitor.OnPingCompleted += CheckChanges;
            hostMonitor.Start();

            CreateWebHostBuilder(args).Build().Run();
        }

        static MonitorContext MonitorContext = null;
        static int count = 0;
        static object lockObject = new object();
        public static async void ResultToDataBase(object sender, PingerEventArgs args)
        {
            lock (lockObject)
            {
                if (count % 100 == 0)
                {
                    if (MonitorContext != null)
                    {
                        MonitorContext.Dispose();
                    }
                    count++;
                    GC.Collect();
                    MonitorContext = UpkServices.ServiceProvider.GetService<MonitorContext>();
                    MonitorContext.ChangeTracker.AutoDetectChangesEnabled = false;
                    MonitorContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    MonitorContext.ChangeTracker.LazyLoadingEnabled = false;
                }
            }
            await MonitorContext.Logs.AddRangeAsync(args.PingResults);
            MonitorContext.SaveChanges();
        }

        static List<Log> localLogs;
        public static void CheckChanges(object sender, PingerEventArgs args)
        {
            if (localLogs == null)
            {
                localLogs = args.PingResults.ToList();
            }
            else
            {
                var result = localLogs.Except(args.PingResults, new LogsComparer());
            }
        }

        class LogsComparer : IEqualityComparer<Log>
        {
            public bool Equals(Log x, Log y)
            {

                //Check whether the compared objects reference the same data.
                if (Object.ReferenceEquals(x, y))
                    return true;

                //Check whether any of the compared objects is null.
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                //Check whether the products' properties are equal.
                return x.IpAddress == y.IpAddress && x.Delay * y.Delay > 0;
            }

            public int GetHashCode(Log log)
            {
                //Check whether the object is null
                if (Object.ReferenceEquals(log, null)) return 0;

                //Get hash code for the Name field if it is not null.
                int hashLogIp = log.IpAddress == null ? 0 : log.IpAddress.GetHashCode();

                //Get hash code for the Code field.
                int hashLogDelay = log.Delay.GetHashCode();

                //Calculate the hash code for the product.
                return hashLogIp ^ hashLogDelay;
            }
        }


        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(@"http://*:10500/");
    }
}
