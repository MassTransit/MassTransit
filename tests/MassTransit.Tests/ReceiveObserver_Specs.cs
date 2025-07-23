﻿namespace MassTransit.Tests
{
    namespace ReceivingObserver_Specs
    {
        using System;
        using System.Threading.Tasks;
        using MassTransit.Configuration;
        using NUnit.Framework;
        using TestFramework;
        using TestFramework.Messages;


        [TestFixture]
        public class Receiving_messages_at_the_endpoint :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_call_the_post_consume_notification()
            {
                var observer = GetObserver();

                using var observerHandle = Bus.ConnectReceiveObserver(observer);

                await InputQueueSendEndpoint.Send(new PingMessage());

                Tuple<ConsumeContext, string> context = await observer.PostConsumed;

                Assert.That(context.Item2, Is.EqualTo(TypeCache<MessageHandler<PingMessage>>.ShortName));
            }

            [Test]
            public async Task Should_call_the_post_receive_notification()
            {
                var observer = GetObserver();

                using var observerHandle = Bus.ConnectReceiveObserver(observer);

                await InputQueueSendEndpoint.Send(new PingMessage());

                await observer.PostReceived;
            }

            [Test]
            public async Task Should_call_the_pre_receive_notification()
            {
                var observer = GetObserver();

                using var observerHandle = Bus.ConnectReceiveObserver(observer);

                await InputQueueSendEndpoint.Send(new PingMessage());

                await observer.PreReceived;
            }

            ReceiveObserver GetObserver()
            {
                return new ReceiveObserver(GetTask<ReceiveContext>(), GetTask<ReceiveContext>(), GetTask<Tuple<ReceiveContext, Exception>>(),
                    GetTask<Tuple<ConsumeContext, string>>(), GetTask<ConsumeContext>());
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                Handled<PingMessage>(configurator);
            }
        }


        [TestFixture]
        public class Receiving_messages_at_the_endpoint_by_consumer :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_call_the_post_consume_notification()
            {
                var observer = GetObserver();

                using (var observerHandle = Bus.ConnectReceiveObserver(observer))
                {
                    await InputQueueSendEndpoint.Send(new PingMessage());

                    Tuple<ConsumeContext, string> context = await observer.PostConsumed;

                    Assert.That(context.Item2, Is.EqualTo(TypeCache<PingConsumerDude>.ShortName));
                }
            }

            ReceiveObserver GetObserver()
            {
                return new ReceiveObserver(GetTask<ReceiveContext>(), GetTask<ReceiveContext>(), GetTask<Tuple<ReceiveContext, Exception>>(),
                    GetTask<Tuple<ConsumeContext, string>>(), GetTask<ConsumeContext>());
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                configurator.Consumer<PingConsumerDude>();
            }


            class PingConsumerDude :
                IConsumer<PingMessage>
            {
                public async Task Consume(ConsumeContext<PingMessage> context)
                {
                }
            }
        }


        [TestFixture]
        public class Receiving_messages_at_the_endpoint_badly :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_call_the_pre_receive_notification()
            {
                var observer = GetObserver();

                using var observerHandle = Bus.ConnectReceiveObserver(observer);

                await InputQueueSendEndpoint.Send(new PingMessage());

                await observer.PreReceived;
            }

            [Test]
            public async Task Should_call_the_consume_fault_notification()
            {
                var observer = GetObserver();

                using var observerHandle = Bus.ConnectReceiveObserver(observer);

                await InputQueueSendEndpoint.Send(new PingMessage());

                await observer.ConsumeFaulted;
            }

            ReceiveObserver GetObserver()
            {
                return new ReceiveObserver(GetTask<ReceiveContext>(), GetTask<ReceiveContext>(), GetTask<Tuple<ReceiveContext, Exception>>(),
                    GetTask<Tuple<ConsumeContext, string>>(), GetTask<ConsumeContext>());
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                Handler<PingMessage>(configurator, _ => throw new IntentionalTestException());
            }
        }


        [TestFixture]
        public class Receiving_messages_at_the_endpoint_with_faulty_middleware :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_call_the_pre_receive_notification()
            {
                var observer = GetObserver();

                using var observerHandle = Bus.ConnectReceiveObserver(observer);

                await InputQueueSendEndpoint.Send(new PingMessage());

                await observer.PreReceived;
            }

            [Test]
            public async Task Should_call_the_receive_fault_notification()
            {
                var observer = GetObserver();

                using var observerHandle = Bus.ConnectReceiveObserver(observer);

                await InputQueueSendEndpoint.Send(new PingMessage());

                await observer.ReceiveFaulted;
            }

            ReceiveObserver GetObserver()
            {
                return new ReceiveObserver(GetTask<ReceiveContext>(), GetTask<ReceiveContext>(), GetTask<Tuple<ReceiveContext, Exception>>(),
                    GetTask<Tuple<ConsumeContext, string>>(), GetTask<ConsumeContext>());
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                configurator.AddPrePipeSpecification(new FilterPipeSpecification<ConsumeContext>(new BadFilter()));

                Handler<PingMessage>(configurator, _ => Task.CompletedTask);
            }


            class BadFilter :
                IFilter<ConsumeContext>
            {
                public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
                {
                    await next.Send(context);

                    throw new IntentionalTestException("Failed to delete message simulation");
                }

                public void Probe(ProbeContext context)
                {
                }
            }
        }


        class ReceiveObserver :
            IReceiveObserver
        {
            readonly TaskCompletionSource<ConsumeContext> _consumeFault;
            readonly TaskCompletionSource<Tuple<ConsumeContext, string>> _postConsume;
            readonly TaskCompletionSource<ReceiveContext> _postReceive;
            readonly TaskCompletionSource<ReceiveContext> _preReceive;
            readonly TaskCompletionSource<Tuple<ReceiveContext, Exception>> _receiveFault;

            public ReceiveObserver(TaskCompletionSource<ReceiveContext> preReceive, TaskCompletionSource<ReceiveContext> postReceive,
                TaskCompletionSource<Tuple<ReceiveContext, Exception>> receiveFault, TaskCompletionSource<Tuple<ConsumeContext, string>> postConsume,
                TaskCompletionSource<ConsumeContext> consumeFault)
            {
                _preReceive = preReceive;
                _postReceive = postReceive;
                _receiveFault = receiveFault;
                _postConsume = postConsume;
                _consumeFault = consumeFault;
            }

            public Task<ReceiveContext> PreReceived => _preReceive.Task;

            public Task<ReceiveContext> PostReceived => _postReceive.Task;

            public Task<Tuple<ConsumeContext, string>> PostConsumed => _postConsume.Task;

            public Task<ConsumeContext> ConsumeFaulted => _consumeFault.Task;

            public Task<Tuple<ReceiveContext, Exception>> ReceiveFaulted => _receiveFault.Task;

            public async Task PreReceive(ReceiveContext context)
            {
                _preReceive.TrySetResult(context);
            }

            public async Task PostReceive(ReceiveContext context)
            {
                _postReceive.TrySetResult(context);
            }

            public async Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
                where T : class
            {
                if(!_postConsume.TrySetResult(Tuple.Create<ConsumeContext, string>(context, consumerType)))
                    LogContext.Warning?.Log("PostConsume was already set");
            }

            public async Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan elapsed, string consumerType, Exception exception)
                where T : class
            {
                if(!_consumeFault.TrySetResult(context))
                    LogContext.Warning?.Log("ConsumeFault was already set: {Exception}", exception);
            }

            public async Task ReceiveFault(ReceiveContext context, Exception exception)
            {
                if(!_receiveFault.TrySetResult(Tuple.Create(context, exception)))
                    LogContext.Warning?.Log("ReceiveFault was already set: {Exception}", exception);;
            }
        }
    }
}
