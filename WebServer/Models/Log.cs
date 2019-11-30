using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebServer.Models
{
    public class Log
    {
        public int Id { get; set; }
        [Required]
        //public string Status { get; set; }
        public int Delay { get; set; }
        public DateTime TimeStamp { get; set; }
        
        [ForeignKey("Host")]
        public string IpAddress { get; set; }
        public Host Host { get; set; }
    }
}
