using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class Host
    {                       
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string IpAddress { get; set; }
        public bool Condition { get; set; }

        public List<Logs> Logs { get; set; }
    }
}
