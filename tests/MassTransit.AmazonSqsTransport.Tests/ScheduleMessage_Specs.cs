namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using NUnit.Framework;


    public class ScheduleMessage_Specs :
        AmazonSqsTestFixture
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

        protected override void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
            configurator.UseAmazonSqsMessageScheduler();
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
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
        AmazonSqsTestFixture
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

        protected override void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
            configurator.UseAmazonSqsMessageScheduler();
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
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


    public class SchedulePublishNoScope_Specs :
        AmazonSqsTestFixture
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

        protected override void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
            configurator.UseAmazonSqsMessageScheduler();
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
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
        AmazonSqsTestFixture
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

            Assert.That(timer.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(3.5)));
        }

        protected override void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
            configurator.UseAmazonSqsMessageScheduler();
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.ScheduleSend(TimeSpan.FromSeconds(5), new SecondMessage());

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
}
