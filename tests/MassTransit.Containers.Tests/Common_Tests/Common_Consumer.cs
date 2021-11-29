namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using DependencyInjection.Registration;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Scenarios;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;
    using Testing;


    public class Common_Consumer<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
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

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<SimpleConsumer>();
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            collection.AddScoped<ISimpleConsumerDependency, SimpleConsumerDependency>();
            collection.AddScoped<AnotherMessageConsumer, AnotherMessageConsumerImpl>();

            return collection;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SimpleConsumer>(BusRegistrationContext);
        }
    }

    public class Common_Consumer_Service_Scope<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_receive_using_the_first_consumer()
        {
            const string name = "Joe";

            await InputQueueSendEndpoint.Send(new SimpleMessageClass(name));

            var lastConsumer = await SimpleConsumer.LastConsumer.OrCanceled(InMemoryTestHarness.TestCancellationToken);
            lastConsumer.ShouldNotBe(null);

            var last = await lastConsumer.Last;
            last.Name
                .ShouldBe(name);

            var wasDisposed = await lastConsumer.Dependency.WasDisposed;
            wasDisposed
                .ShouldBe(true); //Dependency was not disposed");

            lastConsumer.Dependency.SomethingDone
                .ShouldBe(true); //Dependency was disposed before consumer executed");

            var lasterConsumer = await SimplerConsumer.LastConsumer.OrCanceled(InMemoryTestHarness.TestCancellationToken);
            lasterConsumer.ShouldNotBe(null);

            var laster = await lasterConsumer.Last.OrCanceled(InMemoryTestHarness.TestCancellationToken);
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<SimpleConsumer>();
            configurator.AddConsumer<SimplerConsumer>();
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            collection.AddScoped<ISimpleConsumerDependency, SimpleConsumerDependency>();
            collection.AddScoped<AnotherMessageConsumer, AnotherMessageConsumerImpl>();

            return collection;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseServiceScope(ServiceProvider);

            configurator.ConfigureConsumer<SimpleConsumer>(BusRegistrationContext);
            configurator.ConfigureConsumer<SimplerConsumer>(BusRegistrationContext);
        }
    }


    public class Registering_a_consumer_directly_in_the_container<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
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

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            collection.RegisterConsumer<SimpleConsumer>();

            collection.AddScoped<ISimpleConsumerDependency, SimpleConsumerDependency>();
            collection.AddScoped<AnotherMessageConsumer, AnotherMessageConsumerImpl>();

            return collection;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SimpleConsumer>(BusRegistrationContext);
        }
    }


    public class Common_Consumer_Retry<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_only_produce_one_fault()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            Assert.That(await InMemoryTestHarness.Published.SelectAsync<Fault<PingMessage>>().Count(), Is.EqualTo(1));
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<PingConsumer>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseMessageRetry(r => r.Immediate(5));
            configurator.UseMessageScope(ServiceProvider);
            configurator.UseInMemoryOutbox();

            configurator.ConfigureConsumer<PingConsumer>(BusRegistrationContext);
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


    public class Common_Consumer_ConfigureEndpoint<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_only_produce_one_fault()
        {
            await Bus.Publish(new PingMessage());

            Assert.That(await InMemoryTestHarness.Published.Any<Fault<PingMessage>>(), Is.False);
        }

        protected override void ConfigureInMemoryTestHarness(InMemoryTestHarness harness)
        {
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(2);
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddTransient<IConfigureReceiveEndpoint, DoNotPublishFaults>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<PingConsumer>();
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


    public class Common_Consume_Filter<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_use_scope()
        {
            await InputQueueSendEndpoint.Send<SimpleMessageInterface>(new { Name = "test" });

            var result = await TaskCompletionSource.Task;
            Assert.IsNotNull(result);
        }

        protected readonly TaskCompletionSource<MyId> TaskCompletionSource;

        public Common_Consume_Filter()
        {
            TaskCompletionSource = GetTask<MyId>();
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddScoped(_ => new MyId(Guid.NewGuid()))
                .AddSingleton(TaskCompletionSource);
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<SimplerConsumer>();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseConsumeFilter(typeof(CommonScopedConsumeFilter<>), BusRegistrationContext);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SimplerConsumer>(BusRegistrationContext);
        }
    }


    public class CommonScopedConsumeFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        readonly MyId _myId;
        readonly TaskCompletionSource<MyId> _taskCompletionSource;

        public CommonScopedConsumeFilter(TaskCompletionSource<MyId> taskCompletionSource, MyId myId)
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


    public class Common_Consume_FilterScope<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_use_the_same_scope_for_consume_and_send()
        {
            await InputQueueSendEndpoint.Send<EasyA>(new { InVar.CorrelationId });

            await EasyASource.Task;
            await EasyBSource.Task;

            var context = await ScopedContextSource.Task.OrCanceled(InMemoryTestHarness.InactivityToken);

            await context.ConsumeContext.Task.OrCanceled(InMemoryTestHarness.InactivityToken);

            await context.ConsumeContextEasyA.Task.OrCanceled(InMemoryTestHarness.InactivityToken);

            await context.SendContext.Task.OrCanceled(InMemoryTestHarness.InactivityToken);

            Assert.ThrowsAsync<TimeoutException>(async () => await context.ConsumeContextEasyB.Task.OrTimeout(s: 2));
        }

        protected readonly TaskCompletionSource<ConsumeContext<EasyA>> EasyASource;
        protected readonly TaskCompletionSource<ConsumeContext<EasyB>> EasyBSource;
        protected readonly TaskCompletionSource<FilterScopeScopedContext> ScopedContextSource;

        public Common_Consume_FilterScope()
        {
            ScopedContextSource = GetTask<FilterScopeScopedContext>();
            EasyASource = GetTask<ConsumeContext<EasyA>>();
            EasyBSource = GetTask<ConsumeContext<EasyB>>();
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddSingleton(EasyASource)
                .AddSingleton(EasyBSource)
                .AddSingleton(ScopedContextSource)
                .AddScoped<FilterScopeScopedContext>()
                .AddScoped(typeof(FilterScopeScopedConsumeFilter<>))
                .AddScoped(typeof(FilterScopeScopedSendFilter<>));
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<EasyAConsumer>();
            configurator.AddConsumer<EasyBConsumer>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseConsumeFilter(typeof(FilterScopeScopedConsumeFilter<>), BusRegistrationContext);
            configurator.UseSendFilter(typeof(FilterScopeScopedSendFilter<>), BusRegistrationContext);

            configurator.ConfigureConsumer<EasyAConsumer>(BusRegistrationContext);
            configurator.ConfigureConsumer<EasyBConsumer>(BusRegistrationContext);
        }


        protected class EasyAConsumer :
            IConsumer<EasyA>
        {
            readonly TaskCompletionSource<ConsumeContext<EasyA>> _received;
            readonly FilterScopeScopedContext _scopedContext;

            public EasyAConsumer(TaskCompletionSource<ConsumeContext<EasyA>> received, FilterScopeScopedContext scopedContext)
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
            readonly FilterScopeScopedContext _scopedContext;

            public EasyBConsumer(TaskCompletionSource<ConsumeContext<EasyB>> received, FilterScopeScopedContext scopedContext)
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
    }


    public class FilterScopeScopedSendFilter<T> :
        IFilter<SendContext<T>>
        where T : class
    {
        readonly FilterScopeScopedContext _scopedContext;

        public FilterScopeScopedSendFilter(FilterScopeScopedContext scopedContext)
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


    public class FilterScopeScopedContext
    {
        public FilterScopeScopedContext(TaskCompletionSource<FilterScopeScopedContext> taskCompletionSource, InMemoryTestHarness harness)
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


    public class FilterScopeScopedConsumeFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        readonly FilterScopeScopedContext _scopedContext;

        public FilterScopeScopedConsumeFilter(FilterScopeScopedContext scopedContext)
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


    public class Common_Consumer_Endpoint<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_receive_on_the_custom_endpoint()
        {
            const string name = "Joe";

            var sendEndpoint = await Bus.GetSendEndpoint(new Uri("loopback://localhost/custom-endpoint-name"));

            await sendEndpoint.Send(new SimpleMessageClass(name));

            var lastConsumer = await SimplerConsumer.LastConsumer.OrCanceled(InMemoryTestHarness.InactivityToken);
            lastConsumer.ShouldNotBe(null);

            var last = await lastConsumer.Last.OrCanceled(InMemoryTestHarness.InactivityToken);
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddScoped<ISimpleConsumerDependency, SimpleConsumerDependency>()
                .AddScoped<AnotherMessageConsumer, AnotherMessageConsumerImpl>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<SimplerConsumer>()
                .Endpoint(e => e.Name = "custom-endpoint-name");
        }
    }


    public class Common_Consumers_Endpoint<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_receive_on_the_custom_endpoint()
        {
            IRequestClient<PingMessage> client = Bus.CreateRequestClient<PingMessage>(new Uri("queue:shared"));

            await client.GetResponse<PongMessage>(new PingMessage());

            IRequestClient<Request> clientB = Bus.CreateRequestClient<Request>(new Uri("queue:shared"));

            await clientB.GetResponse<Response>(new Request());
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<ConsumerA>(typeof(ConsumerADefinition))
                .Endpoint(x => x.Name = "shared");

            configurator.AddConsumer<ConsumerB>(typeof(ConsumerBDefinition))
                .Endpoint(x => x.Name = "shared");

            configurator.AddConsumer<ConsumerC>(typeof(ConsumerCDefinition));
        }


        class ConsumerA :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Task.CompletedTask;
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


    public class Common_Consumer_ServiceEndpoint<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_handle_the_request()
        {
            IRequestClient<PingMessage> requestClient = Bus.CreateRequestClient<PingMessage>();

            var pingId = NewId.NextGuid();

            Response<PongMessage> response = await requestClient.GetResponse<PongMessage>(new PingMessage(pingId));

            Assert.That(response.Message.CorrelationId, Is.EqualTo(pingId));
        }

        [Test]
        public void Should_just_startup()
        {
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<PingRequestConsumer>();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ServiceInstance(x => x.ConfigureEndpoints(BusRegistrationContext));
        }
    }


    public class Common_Consumer_Connect<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_consume_on_connected_receive_endpoint()
        {
            var handle = Connector.ConnectReceiveEndpoint("easy_consumer", (context, cfg) =>
            {
                cfg.ConfigureConsumer<EasyConsumer>(context);
            });

            await handle.Ready;

            await Bus.Publish(new EasyMessage { CorrelationId = NewId.NextGuid() });

            await MessageCompletion.Task;
        }

        protected readonly TaskCompletionSource<ConsumeContext<EasyMessage>> MessageCompletion;

        public Common_Consumer_Connect()
        {
            MessageCompletion = GetTask<ConsumeContext<EasyMessage>>();
        }

        IReceiveEndpointConnector Connector => ServiceProvider.GetRequiredService<IReceiveEndpointConnector>();

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddSingleton(MessageCompletion);
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<EasyConsumer>();
        }
    }


    public class Common_Consumer_FilterOrder<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_include_container_scope()
        {
            await InputQueueSendEndpoint.Send(new EasyMessage { CorrelationId = NewId.NextGuid() });

            await _messageCompletion.Task;

            await _consumerCompletion.Task;

            await _consumerMessageCompletion.Task;
        }

        readonly TaskCompletionSource<ConsumerConsumeContext<EasyConsumer>> _consumerCompletion;
        readonly TaskCompletionSource<ConsumerConsumeContext<EasyConsumer, EasyMessage>> _consumerMessageCompletion;
        readonly TaskCompletionSource<ConsumeContext<EasyMessage>> _messageCompletion;

        public Common_Consumer_FilterOrder()
        {
            _messageCompletion = GetTask<ConsumeContext<EasyMessage>>();
            _consumerCompletion = GetTask<ConsumerConsumeContext<EasyConsumer>>();
            _consumerMessageCompletion = GetTask<ConsumerConsumeContext<EasyConsumer, EasyMessage>>();
        }

        IFilter<ConsumerConsumeContext<EasyConsumer, EasyMessage>> CreateConsumerMessageFilter()
        {
            return ServiceProvider.GetRequiredService<IFilter<ConsumerConsumeContext<EasyConsumer, EasyMessage>>>();
        }

        IFilter<ConsumerConsumeContext<EasyConsumer>> CreateConsumerFilter()
        {
            return ServiceProvider.GetRequiredService<IFilter<ConsumerConsumeContext<EasyConsumer>>>();
        }

        IFilter<ConsumeContext<EasyMessage>> CreateMessageFilter()
        {
            return ServiceProvider.GetRequiredService<IFilter<ConsumeContext<EasyMessage>>>();
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddSingleton(_messageCompletion)
                .AddSingleton(_consumerCompletion)
                .AddSingleton(_consumerMessageCompletion)
                .AddSingleton<IFilter<ConsumeContext<EasyMessage>>, MessageFilter<EasyMessage, IServiceProvider>>()
                .AddSingleton<IFilter<ConsumerConsumeContext<EasyConsumer>>, ConsumerFilter<EasyConsumer, IServiceProvider>>()
                .AddSingleton<IFilter<ConsumerConsumeContext<EasyConsumer, EasyMessage>>, ConsumerMessageFilter<EasyConsumer, EasyMessage, IServiceProvider>>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<EasyConsumer>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<EasyConsumer>(BusRegistrationContext, x =>
            {
                x.Message<EasyMessage>(m => m.UseFilter(CreateMessageFilter()));

                x.UseFilter(CreateConsumerFilter());

                x.ConsumerMessage<EasyMessage>(m => m.UseFilter(CreateConsumerMessageFilter()));
            });
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


    public class Common_Consumer_ScopedFilterOrder<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_include_container_scope()
        {
            await InputQueueSendEndpoint.Send(new EasyMessage { CorrelationId = NewId.NextGuid() });

            ConsumerConsumeContext<EasyConsumer> consumerContext = await _consumerCompletion.Task;
            var scope = consumerContext.GetPayload<IServiceScope>();

            ConsumerConsumeContext<EasyConsumer, EasyMessage> consumerMessageContext = await _consumerMessageCompletion.Task;
            Assert.AreEqual(scope, consumerMessageContext.GetPayload<IServiceScope>());

            ConsumeContext<EasyMessage> messageContext = await MessageCompletion.Task;
            Assert.AreEqual(scope, messageContext.GetPayload<IServiceScope>());
        }

        readonly TaskCompletionSource<ConsumerConsumeContext<EasyConsumer>> _consumerCompletion;
        readonly TaskCompletionSource<ConsumerConsumeContext<EasyConsumer, EasyMessage>> _consumerMessageCompletion;
        protected readonly TaskCompletionSource<ConsumeContext<EasyMessage>> MessageCompletion;

        public Common_Consumer_ScopedFilterOrder()
        {
            MessageCompletion = GetTask<ConsumeContext<EasyMessage>>();
            _consumerCompletion = GetTask<ConsumerConsumeContext<EasyConsumer>>();
            _consumerMessageCompletion = GetTask<ConsumerConsumeContext<EasyConsumer, EasyMessage>>();
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddSingleton(MessageCompletion);
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<EasyConsumer>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<EasyConsumer>(BusRegistrationContext, x =>
            {
                x.UseFilter(new ConsumerFilter<EasyConsumer>(_consumerCompletion));

                x.ConsumerMessage<EasyMessage>(m => m.UseFilter(new ConsumerMessageFilter<EasyConsumer, EasyMessage>(_consumerMessageCompletion)));
            });
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
                if (context.TryGetPayload(out IServiceScope _))
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
                if (context.TryGetPayload(out IServiceScope _))
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
