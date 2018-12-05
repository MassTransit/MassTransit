using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.SignalR.Sample.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MassTransit.SignalR.Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosticsController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public DiagnosticsController(IHubContext<ChatHub> hubContext, HubLifetimeManager<ChatHub> hubLifetimeManager)
        {
            _hubContext = hubContext;
        }

        // GET: api/Diagnostics
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Diagnostics/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Diagnostics
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Diagnostics/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
