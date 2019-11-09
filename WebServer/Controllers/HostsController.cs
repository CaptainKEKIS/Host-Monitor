using System;
using System.Collections.Generic;
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
        public IEnumerable<Host> GetHosts()
        {
            return _context.Hosts;
        }

        // GET: api/Hosts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHost([FromRoute] int id)
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
        public async Task<IActionResult> PutHost([FromRoute] int id, [FromBody] Host host)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != host.Id)
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

            return CreatedAtAction("GetHost", new { id = host.Id }, host);
        }

        // DELETE: api/Hosts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHost([FromRoute] int id)
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

        private bool HostExists(int id)
        {
            return _context.Hosts.Any(e => e.Id == id);
        }
    }
}