using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Host_Monitor;
using Newtonsoft.Json;
using System.IO;

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
        [HttpGet("{GetSettings}")]
        public ActionResult<Settings> Get()
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
            return settings;
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
