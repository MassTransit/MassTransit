namespace MassTransit.HangfireIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Internals;
    using NUnit.Framework;
    using Scheduling;
    using Util;


    [TestFixture]
    public class Cleanup_Specs :
        HangfireInMemoryTestFixture
    {
        [Test]
        public async Task Should_remove_hash_when_job_cancelled()
        {
            var id = NewId.NextGuid();
            var hashId = id.ToString("N");

            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.UtcNow + TimeSpan.FromSeconds(3), new FirstMessage { Id = id });

            await Task.Delay(1000);

            await Scheduler.CancelScheduledSend(InputQueueAddress, id);

            Assert.That(async () => await _first.OrTimeout(1000), Throws.TypeOf<TimeoutException>());

            await Task.Delay(100);

            var connection = Storage.GetConnection();

            Dictionary<string, string> items = connection.GetAllEntriesFromHash(hashId);

            Assert.Null(items);
        }

        [Test]
        public async Task Should_remove_hash_when_job_executed()
        {
            var id = NewId.NextGuid();
            var hashId = id.ToString("N");

            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.Now, new FirstMessage { Id = id });

            await _first;

            await Task.Delay(100);

            var connection = Storage.GetConnection();

            Dictionary<string, string> items = connection.GetAllEntriesFromHash(hashId);

            Assert.Null(items);
        }

        Task<ConsumeContext<FirstMessage>> _first;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            ScheduleTokenId.UseTokenId<FirstMessage>(x => x.Id);
            _first = Handler<FirstMessage>(configurator, context => TaskUtil.Completed);
        }


        public class FirstMessage
        {
            public Guid Id { get; set; }
        }
    }
}
