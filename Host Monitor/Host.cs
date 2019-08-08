using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Host_Monitor
{
    class Host
    {
        public string Name { get; set; }
        public string IP { get; set; }
        public bool Condition { get; set; }
        public string Status { get; set; }

        public static void WriteHostsToFile(List<Host> Hosts, string PathToFile)
        {
            File.WriteAllText(PathToFile, JsonConvert.SerializeObject(Hosts));
        }
        public static List<Host> ReadHostsFromFile(string PathToFile)
        {
            List<Host> Hosts = JsonConvert.DeserializeObject<List<Host>>(File.ReadAllText(PathToFile));
            return Hosts;
        }
    }
}
