using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Host_Monitor
{
    class Settings
    {
        public string PathToHostsFile { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } //TODO: зашифровать
        public string SenderName { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string MailTo { get; set; }
        public string ReplyTo { get; set; }
        public int TimeOut { get; set; }
        public int DataSize { get; set; }
        public int Ttl { get; set; }
        public int PingInterval { get; set; }

        public  void WriteToFile(Settings settings, string PathToFile)
        {
            File.WriteAllText(PathToFile, JsonConvert.SerializeObject(settings));
        }
        public  Settings ReadFromFile(string PathToFile)
        {
            Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(PathToFile));
            return settings;
        }
    }
}
