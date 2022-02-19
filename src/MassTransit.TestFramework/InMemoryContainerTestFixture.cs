namespace MassTransit.TestFramework
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using Testing;
    using Util;


    public abstract class InMemoryContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        static readonly TestOutputLoggerFactory LoggerFactory;

        static readonly bool _enableLog = !bool.TryParse(Environment.GetEnvironmentVariable("CI"), out var isBuildServer) || !isBuildServer;
        readonly List<Action<CancellationToken>> _cancellation;
        readonly ITestFixtureContainerFactory _containerFactory;
        TestExecutionContext _fixtureContext;
        IServiceScope _serviceScope;

        protected InMemoryTestHarness InMemoryTestHarness;

        static InMemoryContainerTestFixture()
        {
            LoggerFactory = new TestOutputLoggerFactory(_enableLog);
        }

        protected InMemoryContainerTestFixture()
        {
            _containerFactory = new TContainer();
            _cancellation = new List<Action<CancellationToken>>();
        }

        protected IServiceProvider ServiceProvider { get; private set; }

        protected IBusRegistrationContext BusRegistrationContext => ServiceProvider.GetRequiredService<IBusRegistrationContext>();

        protected IBus Bus => InMemoryTestHarness.Bus;
        protected Uri InputQueueAddress => new Uri("queue:input_queue");
        protected ISendEndpoint InputQueueSendEndpoint => InMemoryTestHarness.InputQueueSendEndpoint;

        protected IServiceScope ServiceScope
        {
            get { return _serviceScope ??= ServiceProvider.CreateScope(); }
        }

        /// <summary>
        /// Timeout for the test, used for any delay timers
        /// </summary>
        protected TimeSpan TestTimeout => InMemoryTestHarness.TestTimeout;

        [TearDown]
        public async Task ScopeTearDown()
        {
            if (_serviceScope is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else if (_serviceScope is IDisposable disposable)
                disposable.Dispose();

            _serviceScope = null;
        }

        [OneTimeSetUp]
        public async Task ContainerFixtureOneTimeSetup()
        {
            var collection = _containerFactory.CreateServiceCollection()
                .AddSingleton<ILoggerFactory>(provider => new TestOutputLoggerFactory(true))
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    LoggerFactory.Current = null;

                    ConfigureMassTransit(cfg);
                });

            collection = ConfigureServices(collection);

            ServiceProvider = _containerFactory.BuildServiceProvider(collection);

            ConfigureLogging(ServiceProvider);

            InMemoryTestHarness = ServiceProvider.GetRequiredService<InMemoryTestHarness>();

            ConfigureInMemoryTestHarness(InMemoryTestHarness);

            InMemoryTestHarness.OnConfigureInMemoryBus += ConfigureInMemoryBus;
            InMemoryTestHarness.OnConfigureInMemoryReceiveEndpoint += ConfigureInMemoryReceiveEndpoint;
            InMemoryTestHarness.OnConnectObservers += ConnectObservers;

        #pragma warning disable 4014
            InMemoryTestHarness.TestCancelledTask.ContinueWith(x =>
            {
                _cancellation.ForEach(cb => cb(InMemoryTestHarness.TestCancellationToken));
            });
        #pragma warning restore 4014

            _fixtureContext = TestExecutionContext.CurrentContext;

            LoggerFactory.Current = _fixtureContext;

            await InMemoryTestHarness.Start();
        }

        [OneTimeTearDown]
        public async Task ContainerTestFixtureTearDown()
        {
            LoggerFactory.Current = _fixtureContext;

            LogContext.Debug?.Log("Stopping test harness");

            await InMemoryTestHarness.Stop().ConfigureAwait(false);

            switch (ServiceProvider)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }

        protected virtual void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
        }

        protected virtual IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection;
        }

        protected virtual void ConfigureInMemoryTestHarness(InMemoryTestHarness harness)
        {
        }

        protected virtual void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
        }

        protected virtual void ConnectObservers(IBus bus)
        {
        }

        protected IRequestClient<T> GetRequestClient<T>()
            where T : class
        {
            return ServiceScope.ServiceProvider.GetRequiredService<IRequestClient<T>>();
        }

        protected IClientFactory GetClientFactory()
        {
            return ServiceProvider.GetRequiredService<IClientFactory>();
        }

        protected ISagaRepository<T> GetSagaRepository<T>()
            where T : class, ISaga
        {
            return ServiceProvider.GetRequiredService<ISagaRepository<T>>();
        }

        protected IEndpointNameFormatter EndpointNameFormatter => BusRegistrationContext.EndpointNameFormatter;

        protected async Task<Task<ConsumeContext<T>>> ConnectPublishHandler<T>()
            where T : class
        {
            Task<ConsumeContext<T>> result = null;
            var handle = Bus.ConnectReceiveEndpoint(context =>
            {
                result = Handled<T>(context);
            });

            await handle.Ready;

            return result;
        }

        protected async Task<Task<ConsumeContext<T>>> ConnectPublishHandler<T>(Func<ConsumeContext<T>, bool> filter)
            where T : class
        {
            Task<ConsumeContext<T>> result = null;
            var handle = Bus.ConnectReceiveEndpoint(context =>
            {
                result = Handled(context, filter);
            });

            await handle.Ready;

            return result;
        }

        /// <summary>
        /// Subscribes a message handler to the bus, which is disconnected after the message
        /// is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns>An awaitable task completed when the message is received</returns>
        protected virtual Task<ConsumeContext<T>> SubscribeHandler<T>()
            where T : class
        {
            return InMemoryTestHarness.SubscribeHandler<T>();
        }

        /// <summary>
        /// Subscribes a message handler to the bus, which is disconnected after the message
        /// is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="filter">A filter that only completes the task if filter is true</param>
        /// <returns>An awaitable task completed when the message is received</returns>
        protected virtual Task<ConsumeContext<T>> SubscribeHandler<T>(Func<ConsumeContext<T>, bool> filter)
            where T : class
        {
            return InMemoryTestHarness.SubscribeHandler(filter);
        }

        /// <summary>
        /// Registers a handler on the receive endpoint that is cancelled when the test is canceled
        /// and completed when the message is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The endpoint configurator</param>
        /// <returns></returns>
        protected Task<ConsumeContext<T>> Handled<T>(IReceiveEndpointConfigurator configurator)
            where T : class
        {
            return InMemoryTestHarness.Handled<T>(configurator);
        }

        /// <summary>
        /// Registers a consumer on the receive endpoint that is cancelled when the test is canceled
        /// and completed when the message is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The endpoint configurator</param>
        /// <returns></returns>
        protected Task<ConsumeContext<T>> HandledByConsumer<T>(IReceiveEndpointConfigurator configurator)
            where T : class
        {
            return InMemoryTestHarness.HandledByConsumer<T>(configurator);
        }

        /// <summary>
        /// Registers a handler on the receive endpoint that is cancelled when the test is canceled
        /// and completed when the message is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The endpoint configurator</param>
        /// <param name="filter">Filter the messages based on the handled consume context</param>
        /// <returns></returns>
        protected Task<ConsumeContext<T>> Handled<T>(IReceiveEndpointConfigurator configurator, Func<ConsumeContext<T>, bool> filter)
            where T : class
        {
            return InMemoryTestHarness.Handled(configurator, filter);
        }

        /// <summary>
        /// Registers a handler on the receive endpoint that is cancelled when the test is canceled
        /// and completed when the message is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The endpoint configurator</param>
        /// <param name="expectedCount">The expected number of messages</param>
        /// <returns></returns>
        protected Task<ConsumeContext<T>> Handled<T>(IReceiveEndpointConfigurator configurator, int expectedCount)
            where T : class
        {
            return InMemoryTestHarness.Handled<T>(configurator, expectedCount);
        }

        /// <summary>
        /// Registers a handler on the receive endpoint that is completed after the specified handler is
        /// executed and canceled if the test is canceled.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        protected Task<ConsumeContext<T>> Handler<T>(IReceiveEndpointConfigurator configurator, MessageHandler<T> handler)
            where T : class
        {
            return InMemoryTestHarness.Handler(configurator, handler);
        }

        /// <summary>
        /// Returns a task completion that is automatically canceled when the test is canceled
        /// </summary>
        /// <typeparam name="T">The task type</typeparam>
        /// <returns></returns>
        protected TaskCompletionSource<T> GetTask<T>()
        {
            TaskCompletionSource<T> source = TaskUtil.GetTask<T>();
            _cancellation.Add(token => source.TrySetCanceled(token));

            return source;
        }

        static void ConfigureLogging(IServiceProvider provider)
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

            LogContext.ConfigureCurrentLogContext(loggerFactory);
        }
    }
}
