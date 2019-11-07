using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public partial class Host
    {
        public bool? StatusChanged
        {
            get
            {
                return false;
            }
            set { StatusChanged = value; }
        }
    }
}