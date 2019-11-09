using System.ComponentModel.DataAnnotations.Schema;

namespace Host_Monitor
{
    class Host
    {                                           //оставить Name, IP, Condition. Status убрать в результаты пинга
        public string Name { get; set; }
        public string IP { get; set; }
        public bool Condition { get; set; }
        public string Status { get; set; }

        [NotMapped]
        public bool StatusChanged { get; set; }

        /*
        public static void WriteHostsToFile(List<Host> Hosts, string PathToFile)
        {
            File.WriteAllText(PathToFile, JsonConvert.SerializeObject(Hosts));
        }
        public static List<Host> ReadHostsFromFile(string PathToFile)
        {
            List<Host> Hosts = JsonConvert.DeserializeObject<List<Host>>(File.ReadAllText(PathToFile));
            return Hosts;
        }
        */
    }
}
