namespace MassTransit.AzureServiceBusTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;
    using Util;


    [TestFixture, Explicit]
    public class Renewing_a_lock_on_an_existing_message :
        AzureServiceBusTestFixture
    {
        public Renewing_a_lock_on_an_existing_message()
        {
            TestTimeout = TimeSpan.FromMinutes(3);
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.LockDuration = TimeSpan.FromSeconds(60);

            configurator.UseRenewLock(TimeSpan.FromSeconds(20));

            configurator.Consumer<PingConsumer>();
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
        }

        [Test]
        public async Task Should_complete_the_consumer()
        {
            var context = await PingConsumer.Completed.Task;
        }

        class PingConsumer :
            IConsumer<PingMessage>
        {
            public static TaskCompletionSource<PingMessage> Completed = TaskUtil.GetTask<PingMessage>();

            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await Task.Delay(TimeSpan.FromMinutes(2), context.CancellationToken).ConfigureAwait(false);

                Completed.TrySetResult(context.Message);

            }
        }
    }
}
