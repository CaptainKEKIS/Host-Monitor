using System;
using System.IO;
using HMLib;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace WebServer
{
    public class Program
    {
        public static void Main(string[] args, IConfiguration config)
        {
            
            CreateWebHostBuilder(args).Build().Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            .UseUrls(@"http://*:10500/");

        
    }
}
