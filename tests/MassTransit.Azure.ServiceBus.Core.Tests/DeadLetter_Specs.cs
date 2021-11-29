namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_skipped_messages_to_the_dead_letter_queue :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_complete_the_message()
        {
            var observer = new ReceiveObserver(GetTask<ReceiveContext>());

            Bus.ConnectReceiveObserver(observer);

            var endpoint = await Bus.GetSendEndpoint(new Uri("queue:input_queue_dl"));

            await endpoint.Send(new PingMessage());

            await observer.Completed;
        }

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input_queue_dl", x =>
            {
                x.ConfigureConsumeTopology = false;

                x.ConfigureDeadLetterQueueDeadLetterTransport();
            });
        }


        class ReceiveObserver :
            IReceiveObserver
        {
            readonly TaskCompletionSource<ReceiveContext> _completed;

            public ReceiveObserver(TaskCompletionSource<ReceiveContext> completed)
            {
                _completed = completed;
            }

            public Task Completed => _completed.Task;

            Task IReceiveObserver.PreReceive(ReceiveContext context)
            {
                return Task.CompletedTask;
            }

            Task IReceiveObserver.PostReceive(ReceiveContext context)
            {
                _completed.TrySetResult(context);

                return Task.CompletedTask;
            }

            Task IReceiveObserver.PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
                where T : class
            {
                return Task.CompletedTask;
            }

            Task IReceiveObserver.ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
                where T : class
            {
                return Task.CompletedTask;
            }

            Task IReceiveObserver.ReceiveFault(ReceiveContext context, Exception exception)
            {
                _completed.TrySetException(exception);

                return Task.CompletedTask;
            }
        }
    }


    public class Sending_faulted_messages_to_the_dead_letter_queue :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_complete_the_message()
        {
            var observer = new ReceiveObserver(GetTask<ReceiveContext>());

            Bus.ConnectReceiveObserver(observer);

            var endpoint = await Bus.GetSendEndpoint(new Uri("queue:input_queue_dl"));

            await endpoint.Send(new PongMessage());

            await observer.Completed;
        }

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input_queue_dl", x =>
            {
                x.ConfigureConsumeTopology = false;

                x.ConfigureDeadLetterQueueErrorTransport();

                x.Handler<PongMessage>(async context => throw new IntentionalTestException());
            });
        }


        class ReceiveObserver :
            IReceiveObserver
        {
            readonly TaskCompletionSource<ReceiveContext> _completed;

            public ReceiveObserver(TaskCompletionSource<ReceiveContext> completed)
            {
                _completed = completed;
            }

            public Task Completed => _completed.Task;

            Task IReceiveObserver.PreReceive(ReceiveContext context)
            {
                return Task.CompletedTask;
            }

            Task IReceiveObserver.PostReceive(ReceiveContext context)
            {
                _completed.TrySetResult(context);

                return Task.CompletedTask;
            }

            Task IReceiveObserver.PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
                where T : class
            {
                return Task.CompletedTask;
            }

            Task IReceiveObserver.ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
                where T : class
            {
                return Task.CompletedTask;
            }

            Task IReceiveObserver.ReceiveFault(ReceiveContext context, Exception exception)
            {
                _completed.TrySetException(exception);

                return Task.CompletedTask;
            }
        }
    }
}
