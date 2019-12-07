using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebServer.Models
{
    public partial class Host
    {    
        [Required]
        public string Name { get; set; }
        [Key]
        public string IpAddress { get; set; }
        public bool Condition { get; set; }

        public List<Log> Logs { get; set; }
    }
}
