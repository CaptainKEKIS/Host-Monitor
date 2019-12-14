using System.Collections.Generic;

namespace AdminPanel
{
    public partial class Host
    {    
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public bool Condition { get; set; }

        public Log Log { get; set; }
    }
}
