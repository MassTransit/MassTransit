namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Duplicate_messages_by_id_consumer :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_receive_single_message_within_same_message_id()
        {
            var correlation1 = NewId.NextGuid();

            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.MessageId = correlation1));
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.MessageId = correlation1));
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.MessageId = correlation1));
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.MessageId = correlation1));
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(ctx => ctx.MessageId = correlation1));

            var result = await _tcs.Task.ConfigureAwait(false);

            //Assert.That(count, Is.EqualTo(1));
        }

        readonly TaskCompletionSource<PingMessage> _tcs;

        public Duplicate_messages_by_id_consumer()
        {
            _tcs = GetTask<PingMessage>();
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.LockDuration = TimeSpan.FromMilliseconds(10);
            configurator.Consumer(() => new TestBatchConsumer(_tcs));
        }


        class TestBatchConsumer :
            IConsumer<PingMessage>
        {
            static int _used;
            readonly TaskCompletionSource<PingMessage> _messageTask;

            public TestBatchConsumer(TaskCompletionSource<PingMessage> messageTask)
            {
                _messageTask = messageTask;
            }

            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await Task.Delay(3000);
                // if (Interlocked.Increment(ref _used) == 1)
                //     throw new ArgumentException();

                _messageTask.TrySetResult(context.Message);
            }
        }
    }
}
