using System;
using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class Logs
    {
        public int Id { get; set; }
        [Required]
        public string Status { get; set; }
        public int Delay { get; set; }
        public DateTime TimeStamp { get; set; }
        
        public int HostId { get; set; }
        public Host Host { get; set; }
    }
}
