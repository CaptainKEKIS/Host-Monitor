using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebServer.Models;

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostsController : ControllerBase
    {
        private readonly MonitorContext _context;

        public HostsController(MonitorContext context)
        {
            _context = context;
        }

        // GET: api/Hosts
        [HttpGet]
        public IEnumerable<object>tHosts()
        {
            var maxTime = _context.Logs.Max(t => t.TimeStamp);
            var result = _context.Logs
                .Where(log => log.TimeStamp == maxTime)
                .Select(log => new { log.IpAddress, log.Delay, Name = log.Host.Name, Condition = log.Host.Condition })
                .ToArray();
            /*List<Host> hosts = _context.Hosts.Select(h => new Host
            {
                Name = h.Name,
                IpAddress = h.IpAddress,
                Condition = h.Condition,
                Logs = h.Logs
                .Where(l => l.Id == h.Logs.Max(lm => lm.Id))
                .ToList()
            })
            .ToList();*/
            return result;
        }

        // GET: api/Hosts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHost([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var host = await _context.Hosts.FindAsync(id);

            if (host == null)
            {
                return NotFound();
            }

            return Ok(host);
        }

        // PUT: api/Hosts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHost([FromRoute] string id, [FromBody] Host host)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != host.IpAddress)
            {
                return BadRequest();
            }

            _context.Entry(host).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Hosts
        [HttpPost]
        public async Task<IActionResult> PostHost([FromBody] Host host)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Hosts.Add(host);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHost", new { id = host.IpAddress }, host);
        }

        // DELETE: api/Hosts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHost([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var host = await _context.Hosts.FindAsync(id);
            if (host == null)
            {
                return NotFound();
            }

            _context.Hosts.Remove(host);
            await _context.SaveChangesAsync();

            return Ok(host);
        }

        private bool HostExists(string id)
        {
            return _context.Hosts.Any(e => e.IpAddress == id);
        }
    }
}