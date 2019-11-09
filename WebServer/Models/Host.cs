using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebServer.Models
{
    public partial class Host
    {                       
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string IpAddress { get; set; }
        public bool Condition { get; set; }
        [NotMapped]
        public bool StatusChanged { get; set; }

        public List<Logs> Logs { get; set; }
    }
}
