using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Host_Monitor;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "Settings.json");
            string text;
            try
            {
                using (StreamReader sr = new StreamReader(settingsPath))
                {
                    text = sr.ReadToEnd();
                }
            }
            catch (Exception)
            {
                throw;
            }
            Settings settings = JsonConvert.DeserializeObject<Settings>(text);
            CreateWebHostBuilder(args).Build().Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            .UseUrls("http://0.0.0.0:5000/");

        
    }
}
