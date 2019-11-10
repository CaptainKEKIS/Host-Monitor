using HMLib;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WebServer
{
    public class ConfigHelper
    {
        private static ConfigHelper _appConfig;
        private readonly IConfiguration config;

        public string appSettingValue { get; set; }

        public static string GetDefaultConnection()
        {
            _appConfig = GetConfig();
            return _appConfig.config.GetConnectionString("DefaultConnection");
        }

        public ConfigHelper(IConfiguration config)
        {
            this.config = config;
            var a = config.GetConnectionString("DefaultConnection");
            var b = config.GetSection("UserSettings");
            var c = config.GetValue<Settings>("UserSettings");
        }

        public static ConfigHelper GetConfig()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var settings = new ConfigHelper(configuration);

            return settings;
        }




















        //private static AppSettings _appSettings;
        //private readonly IConfiguration config;

        //public string AppConnection { get; set; }

        //public AppSettings(IConfiguration config)
        //{
        //    this.AppConnection = config.GetValue<string>("DefaultConnection");

        //    // Now set Current
        //    _appSettings = this;
        //    this.config = config;
        //}

        //public object GetProperty(string propName)
        //{
        //    var r = config.GetValue<Settings>(propName);
        //    return r;
        //}

        //public static AppSettings Current
        //{
        //    get
        //    {
        //        if (_appSettings == null)
        //        {
        //            _appSettings = GetCurrentSettings();
        //        }

        //        return _appSettings;
        //    }
        //}

        //public static AppSettings GetCurrentSettings()
        //{
        //    var builder = new ConfigurationBuilder()
        //                    .SetBasePath(Directory.GetCurrentDirectory())
        //                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //                    .AddEnvironmentVariables();

        //    IConfigurationRoot configuration = builder.Build();

        //    var settings = new AppSettings(configuration.GetSection("UserSettings"));

        //    return settings;
        //}
    }
}
