using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Host_Monitor;
using Newtonsoft.Json;
using System.IO;
using System.Net.NetworkInformation;

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

        // GET api/values/GetSettings
        [HttpGet("{PingHost}")]
        public ActionResult<string> Get(string login, string pass)
        {
            string settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "Settings.json");
            string text;
            try
            {
                using (StreamReader sr = new StreamReader(settingsPath))
                {
                    text = sr.ReadToEnd();
                }
            }
            catch (Exception)
            {
                throw;
            }
            Settings settings = JsonConvert.DeserializeObject<Settings>(text);
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
            string Replies = JsonConvert.SerializeObject(pingReply); /////Чота надо сделать ... либо класс либо........
            return Replies;
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
