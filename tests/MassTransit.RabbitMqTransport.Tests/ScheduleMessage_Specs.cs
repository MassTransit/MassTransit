namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    public class ScheduleMessage_Specs :
        RabbitMqTestFixture
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

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();
        }

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

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();
        }

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


    public class Should_schedule_in_the_future_with_headers :
        RabbitMqTestFixture
    {
        Task<ConsumeContext<FirstMessage>> _first;

        Task<ConsumeContext<SecondMessage>> _second;

        [Test]
        public async Task Should_get_both_messages()
        {
            await InputQueueSendEndpoint.Send(new FirstMessage());

            await _first;

            ConsumeContext<SecondMessage> consumeContext = await _second;

            Assert.That(consumeContext.Headers.TryGetHeader("Super-Fun", out var headerValue), Is.True);
            Assert.That(headerValue, Is.EqualTo("Super Fun!"));
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.ScheduleSend<SecondMessage>(TimeSpan.FromSeconds(3), new { __Header_Super_Fun = "Super Fun!" });
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
        Task<ConsumeContext<FirstMessage>> _first;

        Task<ConsumeContext<SecondMessage>> _second;

        [Test]
        public async Task Should_get_both_messages()
        {
            await InputQueueSendEndpoint.Send(new FirstMessage());

            await _first;

            await _second;
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();
        }

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
