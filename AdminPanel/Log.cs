using System;

namespace AdminPanel
{
    public class Log
    {
        public int Id { get; set; }
        public int Delay { get; set; }
        public DateTime TimeStamp { get; set; }
        public string IpAddress { get; set; }
        public Host Host { get; set; }
    }
}
