using System.Collections.Generic;

namespace AdminPanel
{
    public partial class Host
    {
        private int _delay;
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public bool Condition { get; set; }
        public string Status
        {
            get
            {
                if(_delay > 0)
                {
                    return "On rabotaet";
                }
                else
                {
                    return "Off rabotaet";
                }
            }
        }
        public int Delay
        {
            get
            {
                return _delay;
            }
            set
            {
                _delay = value;
            }
        }
    }
}