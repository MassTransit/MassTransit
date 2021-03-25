namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Definition;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using MassTransit.Context;
    using NUnit.Framework;
    using Scenarios;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;
    using Testing;
    using Util;


    public abstract class Common_Consumer :
        InMemoryTestFixture
    {
        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_receive_using_the_first_consumer()
        {
            const string name = "Joe";

            await InputQueueSendEndpoint.Send(new SimpleMessageClass(name));

            var lastConsumer = await SimpleConsumer.LastConsumer;
            lastConsumer.ShouldNotBe(null);

            var last = await lastConsumer.Last;
            last.Name
                .ShouldBe(name);

            var wasDisposed = await lastConsumer.Dependency.WasDisposed;
            wasDisposed
                .ShouldBe(true); //Dependency was not disposed");

            lastConsumer.Dependency.SomethingDone
                .ShouldBe(true); //Dependency was disposed before consumer executed");
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SimpleConsumer>(Registration);
        }
    }


    public abstract class Common_Consumer_Retry :
        InMemoryTestFixture
    {
        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_only_produce_one_fault()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            Assert.That(await InMemoryTestHarness.Published.SelectAsync<Fault<PingMessage>>().Count(), Is.EqualTo(1));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<PingConsumer>(Registration);
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<PingConsumer>();
            configurator.AddBus(provider => BusControl);
        }


        class PingConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                throw new IntentionalTestException();
            }
        }
    }


    public abstract class Common_Consumer_ConfigureEndpoint :
        InMemoryTestFixture
    {
        protected Common_Consumer_ConfigureEndpoint()
        {
            InMemoryTestHarness.TestInactivityTimeout = TimeSpan.FromSeconds(2);
        }

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_only_produce_one_fault()
        {
            await Bus.Publish(new PingMessage());

            Assert.That(await InMemoryTestHarness.Published.Any<Fault<PingMessage>>(), Is.False);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(Registration);
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<PingConsumer>();
            configurator.AddBus(provider => BusControl);
        }


        protected class DoNotPublishFaults :
            IConfigureReceiveEndpoint
        {
            public void Configure(string name, IReceiveEndpointConfigurator configurator)
            {
                configurator.PublishFaults = false;
            }
        }


        class PingConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                throw new IntentionalTestException();
            }
        }
    }


    public abstract class Common_Consume_Filter :
        InMemoryTestFixture
    {
        protected readonly TaskCompletionSource<MyId> TaskCompletionSource;

        protected Common_Consume_Filter()
        {
            TaskCompletionSource = GetTask<MyId>();
        }

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_use_scope()
        {
            await InputQueueSendEndpoint.Send<SimpleMessageInterface>(new {Name = "test"});

            var result = await TaskCompletionSource.Task;
            Assert.IsNotNull(result);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            ConfigureFilter(configurator);
        }

        protected abstract void ConfigureFilter(IConsumePipeConfigurator configurator);

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SimplerConsumer>(Registration);
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<SimplerConsumer>();
            configurator.AddBus(provider => BusControl);
        }


        protected class ScopedFilter<T> :
            IFilter<ConsumeContext<T>>
            where T : class
        {
            readonly MyId _myId;
            readonly TaskCompletionSource<MyId> _taskCompletionSource;

            public ScopedFilter(TaskCompletionSource<MyId> taskCompletionSource, MyId myId)
            {
                _taskCompletionSource = taskCompletionSource;
                _myId = myId;
            }

            public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
            {
                _taskCompletionSource.TrySetResult(_myId);
                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }

        protected class ContextCancellingScopeFilter<T> :
            IFilter<ConsumeContext<T>>
            where T : class
        {
            readonly MyId _myId;
            readonly TaskCompletionSource<MyId> _taskCompletionSource;

            public ContextCancellingScopeFilter(TaskCompletionSource<MyId> taskCompletionSource, MyId myId)
            {
                _taskCompletionSource = taskCompletionSource;
                _myId = myId;
            }

            public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
            {
                _taskCompletionSource.TrySetResult(_myId);
                return next.Send(new CancelledContext(context));
            }

            public void Probe(ProbeContext context) { }

            public class CancelledContext : ConsumeContextProxy<T>
            {
                public override CancellationToken CancellationToken { get; }
                public CancelledContext(ConsumeContext<T> context) : base(context)
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        cts.Cancel();
                        CancellationToken = cts.Token;
                    }
                }
            }
        }
    }


    public abstract class Common_Consume_FilterScope :
        InMemoryTestFixture
    {
        protected readonly TaskCompletionSource<ConsumeContext<EasyA>> EasyASource;
        protected readonly TaskCompletionSource<ConsumeContext<EasyB>> EasyBSource;
        protected readonly TaskCompletionSource<ScopedContext> ScopedContextSource;

        protected Common_Consume_FilterScope()
        {
            ScopedContextSource = GetTask<ScopedContext>();
            EasyASource = GetTask<ConsumeContext<EasyA>>();
            EasyBSource = GetTask<ConsumeContext<EasyB>>();
        }

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_use_the_same_scope_for_consume_and_send()
        {
            await InputQueueSendEndpoint.Send<EasyA>(new {InVar.CorrelationId});

            await EasyASource.Task;
            await EasyBSource.Task;

            var context = await ScopedContextSource.Task.OrCanceled(InactivityToken);

            await context.ConsumeContext.Task.OrCanceled(InactivityToken);

            await context.ConsumeContextEasyA.Task.OrCanceled(InactivityToken);

            await context.SendContext.Task.OrCanceled(InactivityToken);

            Assert.ThrowsAsync<TimeoutException>(async () => await context.ConsumeContextEasyB.Task.OrTimeout(s: 2));
        }

        protected abstract void ConfigureFilters(IInMemoryReceiveEndpointConfigurator configurator);

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            ConfigureFilters(configurator);

            configurator.ConfigureConsumer<EasyAConsumer>(Registration);
            configurator.ConfigureConsumer<EasyBConsumer>(Registration);
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<EasyAConsumer>();
            configurator.AddConsumer<EasyBConsumer>();
            configurator.AddBus(provider => BusControl);
        }


        public class EasyA
        {
            public Guid CorrelationId { get; set; }
        }


        public class EasyB
        {
            public Guid CorrelationId { get; set; }
        }


        protected class EasyAConsumer :
            IConsumer<EasyA>
        {
            readonly TaskCompletionSource<ConsumeContext<EasyA>> _received;
            readonly ScopedContext _scopedContext;

            public EasyAConsumer(TaskCompletionSource<ConsumeContext<EasyA>> received, ScopedContext scopedContext)
            {
                _received = received;
                _scopedContext = scopedContext;
            }

            public async Task Consume(ConsumeContext<EasyA> context)
            {
                await context.Send(context.ReceiveContext.InputAddress, new EasyB());

                _received.TrySetResult(context);
                _scopedContext.ConsumeContextEasyA.TrySetResult(context);
            }
        }


        protected class EasyBConsumer :
            IConsumer<EasyB>
        {
            readonly TaskCompletionSource<ConsumeContext<EasyB>> _received;
            readonly ScopedContext _scopedContext;

            public EasyBConsumer(TaskCompletionSource<ConsumeContext<EasyB>> received, ScopedContext scopedContext)
            {
                _received = received;
                _scopedContext = scopedContext;
            }

            public async Task Consume(ConsumeContext<EasyB> context)
            {
                _received.TrySetResult(context);
                _scopedContext.ConsumeContextEasyB.TrySetResult(context);
            }
        }


        public class ScopedContext
        {
            public ScopedContext(TaskCompletionSource<ScopedContext> taskCompletionSource, AsyncTestHarness harness)
            {
                ConsumeContext = harness.GetTask<ConsumeContext>();
                ConsumeContextEasyA = harness.GetTask<ConsumeContext<EasyA>>();
                ConsumeContextEasyB = harness.GetTask<ConsumeContext<EasyB>>();
                SendContext = harness.GetTask<SendContext>();

                taskCompletionSource.TrySetResult(this);
            }

            public TaskCompletionSource<ConsumeContext> ConsumeContext { get; }
            public TaskCompletionSource<ConsumeContext<EasyA>> ConsumeContextEasyA { get; }
            public TaskCompletionSource<ConsumeContext<EasyB>> ConsumeContextEasyB { get; }
            public TaskCompletionSource<SendContext> SendContext { get; }
        }


        protected class ScopedConsumeFilter<T> :
            IFilter<ConsumeContext<T>>
            where T : class
        {
            readonly ScopedContext _scopedContext;

            public ScopedConsumeFilter(ScopedContext scopedContext)
            {
                _scopedContext = scopedContext;
            }

            public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
            {
                _scopedContext.ConsumeContext.TrySetResult(context);

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        protected class ScopedSendFilter<T> :
            IFilter<SendContext<T>>
            where T : class
        {
            readonly ScopedContext _scopedContext;

            public ScopedSendFilter(ScopedContext scopedContext)
            {
                _scopedContext = scopedContext;
            }

            public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
            {
                _scopedContext.SendContext.TrySetResult(context);

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }


    public abstract class Common_Consumer_Endpoint :
        InMemoryTestFixture
    {
        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_receive_on_the_custom_endpoint()
        {
            const string name = "Joe";

            var sendEndpoint = await Bus.GetSendEndpoint(new Uri("loopback://localhost/custom-endpoint-name"));

            await sendEndpoint.Send(new SimpleMessageClass(name));

            var lastConsumer = await SimplerConsumer.LastConsumer.OrCanceled(TestCancellationToken);
            lastConsumer.ShouldNotBe(null);

            var last = await lastConsumer.Last.OrCanceled(TestCancellationToken);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(Registration);
        }
    }


    public abstract class Common_Consumers_Endpoint :
        InMemoryTestFixture
    {
        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_receive_on_the_custom_endpoint()
        {
            IRequestClient<PingMessage> client = Bus.CreateRequestClient<PingMessage>(new Uri("queue:shared"));

            await client.GetResponse<PongMessage>(new PingMessage());

            IRequestClient<Request> clientB = Bus.CreateRequestClient<Request>(new Uri("queue:shared"));

            await clientB.GetResponse<Response>(new Request());
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<ConsumerA>(typeof(ConsumerADefinition))
                .Endpoint(x => x.Name = "shared");

            configurator.AddConsumer<ConsumerB>(typeof(ConsumerBDefinition))
                .Endpoint(x => x.Name = "shared");

            configurator.AddConsumer<ConsumerC>(typeof(ConsumerCDefinition));

            configurator.AddBus(provider => BusControl);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(Registration);
        }


        class ConsumerA :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return TaskUtil.Completed;
            }
        }


        class ConsumerADefinition :
            ConsumerDefinition<ConsumerA>
        {
        }


        class ConsumerB :
            IConsumer<Request>
        {
            public Task Consume(ConsumeContext<Request> context)
            {
                return context.RespondAsync(new Response());
            }
        }


        class ConsumerBDefinition :
            ConsumerDefinition<ConsumerB>
        {
            public ConsumerBDefinition()
            {
                Endpoint(x => x.Name = "broken");
            }
        }


        class ConsumerC :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            }
        }


        class ConsumerCDefinition :
            ConsumerDefinition<ConsumerC>
        {
            public ConsumerCDefinition()
            {
                Endpoint(e => e.Name = "shared");
            }
        }


        class Request
        {
        }


        class Response
        {
        }
    }


    public abstract class Common_Consumer_ServiceEndpoint :
        InMemoryTestFixture
    {
        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_handle_the_request()
        {
            var serviceClient = Bus.CreateServiceClient();

            IRequestClient<PingMessage> requestClient = serviceClient.CreateRequestClient<PingMessage>();

            var pingId = NewId.NextGuid();

            Response<PongMessage> response = await requestClient.GetResponse<PongMessage>(new PingMessage(pingId));

            Assert.That(response.Message.CorrelationId, Is.EqualTo(pingId));
        }

        [Test]
        public void Should_just_startup()
        {
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ServiceInstance(x => x.ConfigureEndpoints(Registration));
        }
    }


    public abstract class Common_Consumer_Connect :
        InMemoryTestFixture
    {
        protected readonly TaskCompletionSource<ConsumeContext<EasyMessage>> MessageCompletion;

        protected Common_Consumer_Connect()
        {
            MessageCompletion = GetTask<ConsumeContext<EasyMessage>>();
        }

        protected abstract IReceiveEndpointConnector Connector { get; }

        [Test]
        public async Task Should_consume_on_connected_receive_endpoint()
        {
            var handle = Connector.ConnectReceiveEndpoint("easy_consumer", (context, cfg) =>
            {
                cfg.ConfigureConsumer<EasyConsumer>(context);
            });

            await handle.Ready;

            await Bus.Publish(new EasyMessage {CorrelationId = NewId.NextGuid()});

            await MessageCompletion.Task;
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<EasyConsumer>();
            configurator.AddBus(provider => BusControl);
        }
    }


    public abstract class Common_Consumer_FilterOrder :
        InMemoryTestFixture
    {
        protected readonly TaskCompletionSource<ConsumerConsumeContext<EasyConsumer>> ConsumerCompletion;
        protected readonly TaskCompletionSource<ConsumerConsumeContext<EasyConsumer, EasyMessage>> ConsumerMessageCompletion;
        protected readonly TaskCompletionSource<ConsumeContext<EasyMessage>> MessageCompletion;

        protected Common_Consumer_FilterOrder()
        {
            MessageCompletion = GetTask<ConsumeContext<EasyMessage>>();
            ConsumerCompletion = GetTask<ConsumerConsumeContext<EasyConsumer>>();
            ConsumerMessageCompletion = GetTask<ConsumerConsumeContext<EasyConsumer, EasyMessage>>();
        }

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_include_container_scope()
        {
            await InputQueueSendEndpoint.Send(new EasyMessage {CorrelationId = NewId.NextGuid()});

            await MessageCompletion.Task;

            await ConsumerCompletion.Task;

            await ConsumerMessageCompletion.Task;
        }

        protected abstract IFilter<ConsumerConsumeContext<EasyConsumer>> CreateConsumerFilter();
        protected abstract IFilter<ConsumerConsumeContext<EasyConsumer, EasyMessage>> CreateConsumerMessageFilter();
        protected abstract IFilter<ConsumeContext<EasyMessage>> CreateMessageFilter();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<EasyConsumer>(Registration, x =>
            {
                x.Message<EasyMessage>(m => m.UseFilter(CreateMessageFilter()));

                x.UseFilter(CreateConsumerFilter());

                x.ConsumerMessage<EasyMessage>(m => m.UseFilter(CreateConsumerMessageFilter()));
            });
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<EasyConsumer>();
            configurator.AddBus(provider => BusControl);
        }


        protected class ConsumerFilter<TConsumer, TScope> :
            IFilter<ConsumerConsumeContext<TConsumer>>
            where TConsumer : class, IConsumer
            where TScope : class
        {
            readonly TaskCompletionSource<ConsumerConsumeContext<TConsumer>> _taskCompletionSource;

            public ConsumerFilter(TaskCompletionSource<ConsumerConsumeContext<TConsumer>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Send(ConsumerConsumeContext<TConsumer> context, IPipe<ConsumerConsumeContext<TConsumer>> next)
            {
                if (context.TryGetPayload(out TScope _))
                    _taskCompletionSource.TrySetResult(context);
                else
                    _taskCompletionSource.TrySetException(new PayloadException("Service Provider not found"));

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        protected class ConsumerMessageFilter<TConsumer, TMessage, TScope> :
            IFilter<ConsumerConsumeContext<TConsumer, TMessage>>
            where TConsumer : class
            where TMessage : class
            where TScope : class
        {
            readonly TaskCompletionSource<ConsumerConsumeContext<TConsumer, TMessage>> _taskCompletionSource;

            public ConsumerMessageFilter(TaskCompletionSource<ConsumerConsumeContext<TConsumer, TMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Send(ConsumerConsumeContext<TConsumer, TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            {
                if (context.TryGetPayload(out TScope _))
                    _taskCompletionSource.TrySetResult(context);
                else
                    _taskCompletionSource.TrySetException(new PayloadException("Service Provider not found"));

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        protected class MessageFilter<TMessage, TScope> :
            IFilter<ConsumeContext<TMessage>>
            where TMessage : class
            where TScope : class
        {
            readonly TaskCompletionSource<ConsumeContext<TMessage>> _taskCompletionSource;

            public MessageFilter(TaskCompletionSource<ConsumeContext<TMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
            {
                if (context.TryGetPayload(out TScope _))
                    _taskCompletionSource.TrySetException(new PayloadException("Service Provider should not be present"));
                else
                    _taskCompletionSource.TrySetResult(context);

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }


    public abstract class Common_Consumer_ScopedFilterOrder<TScope> :
        InMemoryTestFixture
        where TScope : class
    {
        readonly TaskCompletionSource<ConsumerConsumeContext<EasyConsumer>> _consumerCompletion;
        readonly TaskCompletionSource<ConsumerConsumeContext<EasyConsumer, EasyMessage>> _consumerMessageCompletion;
        protected readonly TaskCompletionSource<ConsumeContext<EasyMessage>> MessageCompletion;

        protected Common_Consumer_ScopedFilterOrder()
        {
            MessageCompletion = GetTask<ConsumeContext<EasyMessage>>();
            _consumerCompletion = GetTask<ConsumerConsumeContext<EasyConsumer>>();
            _consumerMessageCompletion = GetTask<ConsumerConsumeContext<EasyConsumer, EasyMessage>>();
        }

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_include_container_scope()
        {
            await InputQueueSendEndpoint.Send(new EasyMessage {CorrelationId = NewId.NextGuid()});

            ConsumerConsumeContext<EasyConsumer> consumerContext = await _consumerCompletion.Task;
            var scope = consumerContext.GetPayload<TScope>();

            ConsumerConsumeContext<EasyConsumer, EasyMessage> consumerMessageContext = await _consumerMessageCompletion.Task;
            Assert.AreEqual(scope, consumerMessageContext.GetPayload<TScope>());

            ConsumeContext<EasyMessage> messageContext = await MessageCompletion.Task;
            Assert.AreEqual(scope, messageContext.GetPayload<TScope>());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<EasyConsumer>(Registration, x =>
            {
                x.UseFilter(new ConsumerFilter<EasyConsumer>(_consumerCompletion));

                x.ConsumerMessage<EasyMessage>(m => m.UseFilter(new ConsumerMessageFilter<EasyConsumer, EasyMessage>(_consumerMessageCompletion)));
            });
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<EasyConsumer>();
            configurator.AddBus(provider => BusControl);
        }


        class ConsumerFilter<TConsumer> :
            IFilter<ConsumerConsumeContext<TConsumer>>
            where TConsumer : class, IConsumer
        {
            readonly TaskCompletionSource<ConsumerConsumeContext<TConsumer>> _taskCompletionSource;

            public ConsumerFilter(TaskCompletionSource<ConsumerConsumeContext<TConsumer>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Send(ConsumerConsumeContext<TConsumer> context, IPipe<ConsumerConsumeContext<TConsumer>> next)
            {
                if (context.TryGetPayload(out TScope _))
                    _taskCompletionSource.TrySetResult(context);
                else
                    _taskCompletionSource.TrySetException(new PayloadException("Service Provider not found"));

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        class ConsumerMessageFilter<TConsumer, TMessage> :
            IFilter<ConsumerConsumeContext<TConsumer, TMessage>>
            where TConsumer : class
            where TMessage : class
        {
            readonly TaskCompletionSource<ConsumerConsumeContext<TConsumer, TMessage>> _taskCompletionSource;

            public ConsumerMessageFilter(TaskCompletionSource<ConsumerConsumeContext<TConsumer, TMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Send(ConsumerConsumeContext<TConsumer, TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            {
                if (context.TryGetPayload(out TScope _))
                    _taskCompletionSource.TrySetResult(context);
                else
                    _taskCompletionSource.TrySetException(new PayloadException("Service Provider not found"));

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }


    public class EasyMessage
    {
        public Guid CorrelationId { get; set; }
    }


    public class EasyConsumer :
        IConsumer<EasyMessage>
    {
        readonly TaskCompletionSource<ConsumeContext<EasyMessage>> _received;

        public EasyConsumer(TaskCompletionSource<ConsumeContext<EasyMessage>> received)
        {
            _received = received;
        }

        public async Task Consume(ConsumeContext<EasyMessage> context)
        {
            _received.TrySetResult(context);
        }
    }
}
