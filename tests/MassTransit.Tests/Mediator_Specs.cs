namespace MassTransit.Tests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class Delivering_a_message_via_the_mediator :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_able_to_add_consumer_post_creation()
        {
            var mediator = MassTransit.Bus.Factory.CreateMediator(cfg =>
            {
            });

            TaskCompletionSource<ConsumeContext<PingMessage>> received = GetTask<ConsumeContext<PingMessage>>();

            var handle = mediator.ConnectHandler<PingMessage>(x =>
            {
                received.SetResult(x);

                return Task.CompletedTask;
            });

            await mediator.Publish(new PingMessage());

            ConsumeContext<PingMessage> context = await received.Task;

            handle.Disconnect();

            await mediator.Publish(new PingMessage());
        }

        [Test]
        public async Task Should_deliver_the_message()
        {
            var multiConsumer = new MultiTestConsumer(TimeSpan.FromSeconds(8));
            ReceivedMessageList<PingMessage> received = multiConsumer.Consume<PingMessage>();

            var mediator = MassTransit.Bus.Factory.CreateMediator(cfg =>
            {
                multiConsumer.Configure(cfg);
            });

            await mediator.Send(new PingMessage());

            Assert.That(received.Select().Any(), Is.True);
        }

        [Test]
        public async Task Should_deliver_the_publish()
        {
            var multiConsumer = new MultiTestConsumer(TimeSpan.FromSeconds(8));
            ReceivedMessageList<PingMessage> received = multiConsumer.Consume<PingMessage>();

            var mediator = MassTransit.Bus.Factory.CreateMediator(cfg =>
            {
                multiConsumer.Configure(cfg);
            });

            await mediator.Publish(new PingMessage());

            Assert.That(received.Select().Any(), Is.True);
        }

        [Test]
        public async Task Should_fault_at_the_send()
        {
            var mediator = MassTransit.Bus.Factory.CreateMediator(cfg =>
            {
                cfg.Handler<PingMessage>(context => throw new IntentionalTestException("No thank you!"));
            });

            Assert.That(async () => await mediator.Send(new PingMessage()), Throws.TypeOf<IntentionalTestException>());
        }

        [Test]
        public async Task Should_support_send_cancellation()
        {
            using var source = new CancellationTokenSource();

            var mediator = MassTransit.Bus.Factory.CreateMediator(cfg =>
            {
                cfg.Handler<PingMessage>(async context =>
                {
                    source.Cancel();

                    await Task.Delay(5000, context.CancellationToken);
                });
            });

            var pingMessage = new PingMessage();

            Assert.That(async () => await mediator.Send(pingMessage, source.Token), Throws.TypeOf<TaskCanceledException>());
        }

        [Test]
        public async Task Should_handle_request_response()
        {
            var mediator = MassTransit.Bus.Factory.CreateMediator(cfg =>
            {
                cfg.Handler<PingMessage>(context => context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
            });

            IRequestClient<PingMessage> client = mediator.CreateRequestClient<PingMessage>();

            var pingMessage = new PingMessage();

            Response<PongMessage> response = await client.GetResponse<PongMessage>(pingMessage);

            Assert.That(response.Message.CorrelationId, Is.EqualTo(pingMessage.CorrelationId));
        }

        [Test]
        public async Task Should_handle_request_response_cancellation()
        {
            using var source = new CancellationTokenSource();

            var mediator = MassTransit.Bus.Factory.CreateMediator(cfg =>
            {
                cfg.Handler<PingMessage>(async context =>
                {
                    source.Cancel();

                    await Task.Yield();

                    await Task.Delay(5000, context.CancellationToken);

                    await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
                });
            });

            IRequestClient<PingMessage> client = mediator.CreateRequestClient<PingMessage>();

            var pingMessage = new PingMessage();

            Assert.That(async () => await client.GetResponse<PongMessage>(pingMessage, source.Token), Throws.InstanceOf<OperationCanceledException>());
        }

        [Test]
        public async Task Should_not_throw_when_publishing_without_consumer()
        {
            var mediator = MassTransit.Bus.Factory.CreateMediator(cfg =>
            {
            });

            await mediator.Publish(new PingMessage());
        }

        [Test]
        public async Task Should_throw_when_publishing_with_mandatory_without_consumer()
        {
            var mediator = MassTransit.Bus.Factory.CreateMediator(cfg =>
            {
            });

            Assert.ThrowsAsync<MessageNotConsumedException>(() => mediator.Publish(new PingMessage(), context => context.Mandatory = true));
        }
    }


    public class Mediator_performance
    {
        [Test]
        [Explicit]
        public async Task Send()
        {
            var mediator = Bus.Factory.CreateMediator(cfg =>
            {
                cfg.Handler<PingMessage>(context => Task.CompletedTask);
            });

            var message = new PingMessage();

            var timer = Stopwatch.StartNew();

            const int count = 200000;
            const int split = 20;

            for (var i = 0; i < count / split; i++)
                await Task.WhenAll(Enumerable.Range(0, split).Select(_ => mediator.Send(message)));

            timer.Stop();

            Console.WriteLine("Time to process {0} messages = {1}", count, timer.ElapsedMilliseconds + "ms");
            Console.WriteLine("Messages per second: {0}", count * 1000 / timer.ElapsedMilliseconds);
        }

        [Test]
        [Explicit]
        public async Task Request_client()
        {
            var mediator = Bus.Factory.CreateMediator(cfg =>
            {
                cfg.Handler<PingMessage>(context => context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
            });

            IRequestClient<PingMessage> client = mediator.CreateRequestClient<PingMessage>();

            var message = new PingMessage();

            var timer = Stopwatch.StartNew();

            const int count = 200000;
            const int split = 10;

            for (var i = 0; i < count / split; i++)
            {
                async Task<Response<PongMessage>> MakeRequest(int _)
                {
                    return await client.GetResponse<PongMessage>(message);
                }

                await Task.WhenAll(Enumerable.Range(0, split).Select(MakeRequest));
            }

            timer.Stop();

            Console.WriteLine("Time to process {0} messages = {1}", count * 2, timer.ElapsedMilliseconds + "ms");
            Console.WriteLine("Requests per second: {0}", count * 1000 / timer.ElapsedMilliseconds);
        }
    }
}
