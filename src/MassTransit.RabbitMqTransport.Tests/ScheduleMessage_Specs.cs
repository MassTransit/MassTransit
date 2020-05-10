namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using NUnit.Framework;


    public class ScheduleMessage_Specs :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_get_both_messages()
        {
            await InputQueueSendEndpoint.Send(new FirstMessage());

            await _first;

            await _second;
        }

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            base.ConfigureRabbitMqBusHost(configurator, host);

            configurator.UseDelayedExchangeMessageScheduler();
        }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.ScheduleSend(DateTime.Now, new SecondMessage());
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
        RabbitMqTestFixture
    {
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

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            base.ConfigureRabbitMqBusHost(configurator, host);

            configurator.UseDelayedExchangeMessageScheduler();
        }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.ScheduleSend(TimeSpan.FromSeconds(3), new SecondMessage());
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


    public class Scheduling_a_published_message :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_get_both_messages()
        {
            await InputQueueSendEndpoint.Send(new FirstMessage());

            await _first;

            await _second;
        }

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            base.ConfigureRabbitMqBusHost(configurator, host);

            configurator.UseDelayedExchangeMessageScheduler();
        }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.SchedulePublish(TimeSpan.FromSeconds(1), new SecondMessage());
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
