namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Scheduling;


    public class ScheduleMessage_Specs :
        ActiveMqTestFixture
    {
        Task<ConsumeContext<FirstMessage>> _first;

        Task<ConsumeContext<SecondMessage>> _second;

        [Test]
        public async Task Should_get_both_messages()
        {
            await InputQueueSendEndpoint.Send(new FirstMessage());

            await _first;

            await _second;
        }

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();
        }

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.ScheduleSend(DateTime.Now, new SecondMessage());

                await context.ReceiveContext.ReceiveCompleted;
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


    public class SchedulePublish_Specs :
        ActiveMqTestFixture
    {
        Task<ConsumeContext<FirstMessage>> _first;

        Task<ConsumeContext<SecondMessage>> _second;

        [Test]
        public async Task Should_get_both_messages()
        {
            await InputQueueSendEndpoint.Send(new FirstMessage());

            await _first;

            await _second;
        }

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();
        }

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.SchedulePublish(DateTime.Now, new SecondMessage());

                await context.ReceiveContext.ReceiveCompleted;
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


    public class Should_schedule_in_the_future :
        ActiveMqTestFixture
    {
        Task<ConsumeContext<FirstMessage>> _first;

        Task<ConsumeContext<SecondMessage>> _second;

        [Test]
        public async Task Should_get_both_messages()
        {
            await InputQueueSendEndpoint.Send(new FirstMessage());

            await _first;

            var timer = Stopwatch.StartNew();

            await _second;

            timer.Stop();

            Assert.That(timer.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(2)));
        }

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();
        }

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.ScheduleSend(TimeSpan.FromSeconds(3), new SecondMessage());

                await context.ReceiveContext.ReceiveCompleted;
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


    public class Should_not_schedule_subsequent_messages :
        ActiveMqTestFixture
    {
        Task<ConsumeContext<FirstMessage>> _first;

        Task<ConsumeContext<SecondMessage>> _second;

        [Test]
        public async Task Should_get_both_messages()
        {
            var scheduler = new MessageScheduler(new DelayedScheduleMessageProvider(Bus), Bus.Topology);

            await scheduler.ScheduleSend(InputQueueAddress, TimeSpan.FromSeconds(3), new FirstMessage());

            await _first;

            var timer = Stopwatch.StartNew();

            await _second;

            timer.Stop();

            Assert.That(timer.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();
        }

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.Send(InputQueueAddress, new SecondMessage());
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


    [TestFixture(ActiveMqHostAddress.ActiveMqScheme)]
    [TestFixture(ActiveMqHostAddress.AmqpScheme)]
    public class Scheduling_a_published_message_using_quartz :
        ActiveMqTestFixture
    {
        [Test]
        public async Task Should_get_the_message()
        {
            await InputQueueSendEndpoint.Send(new FirstMessage());

            await _first;

            await _second;
        }

        TimeSpan _testOffset;

        public Scheduling_a_published_message_using_quartz(string protocol)
            : base(protocol)
        {
            _testOffset = TimeSpan.Zero;
        }

        Uri QuartzAddress { get; set; }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.SchedulePublish(TimeSpan.FromSeconds(1), new SecondMessage());
            });

            _second = Handled<SecondMessage>(configurator);
        }

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            QuartzAddress = configurator.UseInMemoryScheduler();
        }


        public class FirstMessage
        {
        }


        public class SecondMessage
        {
        }
    }

}
