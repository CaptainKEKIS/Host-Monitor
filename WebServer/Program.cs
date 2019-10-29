using System;
using System.IO;
using HMLib;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
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
            .UseUrls(@"http://*:10500/");

        
    }
}
