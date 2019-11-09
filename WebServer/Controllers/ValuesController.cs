using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Net.NetworkInformation;
using HMLib;
using Microsoft.Extensions.Configuration;

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        //[HttpGet]
        //public ActionResult<IEnumerable<string>> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        private readonly Models.MonitorContext _context;

        public ValuesController(Models.MonitorContext context)
        {
            
            _context = context;
        }

        // GET /api/values/PingHost?login=1&pass=1
        [HttpGet("{PingHost}")]
        public ActionResult<object> Get(string login, string pass/*, IConfiguration config*/)
        {
            var result = _context.Hosts
                .Where(x => x.Id == 1)
                .FirstOrDefault();
            return result;
            /*
            Settings settings = JsonConvert.DeserializeObject<Settings>(config.GetSection("UserSettings").Value);
            string hostsPath = settings.PathToHostsFile;
            List<Host> hosts = Host.ReadHostsFromFile(hostsPath);//сделать шоб была глобальной
            List<PingReply> pingReply = new List<PingReply>();

            Pinger SendPing = new Pinger
            {
                DataSize = settings.DataSize,
                TimeOut = settings.TimeOut,
                Ttl = settings.Ttl
            };

            int count = hosts.Count;
            Task[] tasks = new Task[count];
            int taskIndex = -1;

            foreach (Host host in hosts)
            {
                taskIndex++;
                tasks[taskIndex] = Task.Factory.StartNew(() =>
                {
                    pingReply.Add(SendPing.Ping(host.IP));
                });
            }
            Task.WaitAll(tasks);
            string Replies = JsonConvert.SerializeObject(pingReply);
            return Replies;
            */
        }
        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
