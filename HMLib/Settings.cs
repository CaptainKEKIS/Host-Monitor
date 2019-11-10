using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HMLib
{
    public class Settings
    {
        public string Email { get; set; }
        public string Password { get; set; } //TODO: зашифровать)
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SenderName { get; set; }
        public string ReplyTo { get; set; }
        public string MailTo { get; set; }
        public int TimeOut { get; set; }
        public int PingInterval { get; set; }
        public int DataSize { get; set; }
        public int Ttl { get; set; }
    }
}