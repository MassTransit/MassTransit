namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Serialization;


    [TestFixture]
    public class ScheduleMessage_Specs :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_get_both_messages()
        {
            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.Now, new FirstMessage());

            await _first;

            await AdvanceTime(TimeSpan.FromSeconds(10));

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
                await context.ScheduleSend(TimeSpan.FromSeconds(10), new SecondMessage());
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
    public class ScheduleMessageUsingBson_Specs :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_get_both_messages()
        {
            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.Now, new FirstMessage());

            await _first;

            await AdvanceTime(TimeSpan.FromSeconds(10));

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
                await context.ScheduleSend(TimeSpan.FromSeconds(10), new SecondMessage());
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
    public class ScheduleMessageUsingEncryptedV2_Specs :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_get_both_messages()
        {
            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.Now, new FirstMessage());

            await _first;

            await AdvanceTime(TimeSpan.FromSeconds(10));

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

            var keyProvider = new ConstantSecureKeyProvider(Encoding.UTF8.GetBytes("SHHH, SECRET"));

            var streamProvider = new AesCryptoStreamProviderV2(keyProvider);
            configurator.UseEncryptedSerializerV2(streamProvider);

            configurator.UseBsonSerializer();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                _firstActivityId = Activity.Current?.Id;
                await context.ScheduleSend(TimeSpan.FromSeconds(10), new SecondMessage());
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
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_include_it_with_the_final_message()
        {
            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.Now, new FirstMessage());

            await _first;

            await AdvanceTime(TimeSpan.FromSeconds(10));

            ConsumeContext<SecondMessage> second = await _second;

            Assert.That(second.ExpirationTime.HasValue, Is.True);
            Assert.That(second.ExpirationTime.Value, Is.GreaterThan(DateTime.UtcNow + TimeSpan.FromSeconds(20)));
        }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.ScheduleSend(TimeSpan.FromSeconds(10), new SecondMessage(),
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
