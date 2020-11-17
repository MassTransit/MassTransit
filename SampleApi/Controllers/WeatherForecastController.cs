using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration.Outbox;
using MassTransit.Transports.Outbox;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SampleApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly OutboxDbContext _db;
        //private readonly DbConnection _conn;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        //private readonly IOutboxDbTransactionContext _transactionContext;

        public WeatherForecastController(
            OutboxDbContext db,
            //DbConnection conn,
            ILogger<WeatherForecastController> logger,
            IPublishEndpoint publishEndpoint
            //IOutboxDbTransactionContext transactionContext
            )
        {
            _db = db;
            //_conn = conn;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            //_transactionContext = transactionContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post(int quantity = 10)
        {
            for (int i = 0; i < quantity; i++)
            {
                await _publishEndpoint.Publish(new MyMessage { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow });
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        //[HttpPut]
        //public async Task<IActionResult> Put(int quantity = 10)
        //{
        //    await _conn.OpenAsync();
        //    using var transaction = _conn.BeginTransaction(_transactionContext);

        //    for (int i = 0; i < quantity; i++)
        //    {
        //        await _publishEndpoint.Publish(new MyMessage { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow });
        //    }

        //    transaction.Commit();

        //    return Ok();
        //}
    }

    public class MyMessage
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
