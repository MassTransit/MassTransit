namespace MassTransit.WebJobs.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.WebJobs;
    using Moq;
    using NUnit.Framework;
    using ServiceBusIntegration;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Creating_a_brokered_message_receiver
    {
        [Test]
        public async Task Should_create_the_brokered_message_receiver()
        {
            var message = new Mock<Message>();

            var binder = new Mock<IBinder>();

            var handler = Bus.Factory.CreateBrokeredMessageReceiver(binder.Object, cfg =>
            {
                cfg.CancellationToken = CancellationToken.None;
                cfg.InputAddress = new Uri("sb://masstransit-build.servicebus.windows.net/input-queue");

                cfg.UseRetry(x => x.Intervals(10, 100, 500, 1000));
                cfg.Consumer(() => new Consumer());
            });

            Console.WriteLine(handler.GetProbeResult().ToJsonString());

//            await handler.Handle(message.Object);
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                TestContext.Out.WriteLine("Hello");

                return Task.CompletedTask;
            }
        }
    }
}
