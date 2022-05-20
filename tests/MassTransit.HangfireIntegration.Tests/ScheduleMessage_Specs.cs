namespace MassTransit.HangfireIntegration.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class ScheduleMessage_Specs :
        HangfireInMemoryTestFixture
    {
        [Test]
        public async Task Should_get_both_messages()
        {
            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.Now, new FirstMessage());

            await _first;

            await _second;

            if (_secondActivityId != null && _firstActivityId != null)
                Assert.That(_secondActivityId.StartsWith(_firstActivityId), Is.True);
        }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;
        string _firstActivityId;
        string _secondActivityId;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                _firstActivityId = Activity.Current?.Id;
                await context.ScheduleSend(TimeSpan.FromSeconds(5), new SecondMessage());
            });

            _second = Handler<SecondMessage>(configurator, async context =>
            {
                _secondActivityId = Activity.Current?.Id;
            });
        }


        public class FirstMessage
        {
        }


        public class SecondMessage
        {
        }
    }


    [TestFixture]
    public class ScheduleMessageBson_Specs :
        HangfireInMemoryTestFixture
    {
        [Test]
        public async Task Should_get_both_messages()
        {
            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.Now, new FirstMessage());

            await _first;

            await _second;

            if (_secondActivityId != null && _firstActivityId != null)
                Assert.That(_secondActivityId.StartsWith(_firstActivityId), Is.True);
        }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;
        string _firstActivityId;
        string _secondActivityId;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.UseBsonSerializer();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                _firstActivityId = Activity.Current?.Id;
                await context.ScheduleSend(TimeSpan.FromSeconds(5), new SecondMessage());
            });

            _second = Handler<SecondMessage>(configurator, async context =>
            {
                _secondActivityId = Activity.Current?.Id;
            });
        }


        public class FirstMessage
        {
        }


        public class SecondMessage
        {
        }
    }


    [TestFixture]
    public class Specifying_an_expiration_time :
        HangfireInMemoryTestFixture
    {
        [Test]
        public async Task Should_include_it_with_the_final_message()
        {
            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.Now, new FirstMessage());

            await _first;

            ConsumeContext<SecondMessage> second = await _second;

            Assert.That(second.ExpirationTime.HasValue, Is.True);
            Assert.That(second.ExpirationTime.Value, Is.GreaterThan(DateTime.UtcNow + TimeSpan.FromSeconds(24)));
        }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.ScheduleSend(TimeSpan.FromSeconds(5), new SecondMessage(),
                    Pipe.Execute<SendContext>(x => x.TimeToLive = TimeSpan.FromSeconds(30)));
            });

            _second = Handled<SecondMessage>(configurator);
        }


        public class FirstMessage
        {
        }


        public class SecondMessage
        {
        }
    }
}
