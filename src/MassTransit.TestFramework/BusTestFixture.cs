namespace MassTransit.TestFramework
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Testing;


    /// <summary>
    /// A bus text fixture includes a single bus instance with one or more receiving endpoints.
    /// </summary>
    public abstract class BusTestFixture :
        AsyncTestFixture
    {
        static int _subscribedObserver;
        static readonly bool _enableLog = !bool.TryParse(Environment.GetEnvironmentVariable("CI"), out var isBuildServer) || !isBuildServer;
        static readonly bool _enableDiagnostics = bool.TryParse(Environment.GetEnvironmentVariable("DIAG"), out var enable) && enable;
        public static readonly TestOutputLoggerFactory LoggerFactory;

        static BusTestFixture()
        {
            LoggerFactory = new TestOutputLoggerFactory(_enableLog);
        }

        protected BusTestFixture(BusTestHarness harness)
            : base(harness)
        {
            BusTestHarness = harness;

            harness.OnConnectObservers += ConnectObservers;
            harness.OnConfigureBus += ConfigureBusDiagnostics;
        }

        protected BusTestHarness BusTestHarness { get; }

        protected IBus Bus => BusTestHarness.Bus;
        protected IBusControl BusControl => BusTestHarness.BusControl;

        public static bool IsLogEnabled => _enableLog;

        public static void ConfigureBusDiagnostics(IBusFactoryConfigurator configurator)
        {
            if (_enableLog)
                LogContext.ConfigureCurrentLogContext(LoggerFactory);

            LoggerFactory.Current = default;

            if (_enableDiagnostics)
            {
                if (Interlocked.CompareExchange(ref _subscribedObserver, 1, 0) == 0)
                    DiagnosticListener.AllListeners.Subscribe(new DiagnosticListenerObserver());
            }
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
            return BusTestHarness.SubscribeHandler<T>();
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
            return BusTestHarness.SubscribeHandler(filter);
        }

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
        /// Registers a handler on the receive endpoint that is cancelled when the test is canceled
        /// and completed when the message is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The endpoint configurator</param>
        /// <returns></returns>
        protected Task<ConsumeContext<T>> Handled<T>(IReceiveEndpointConfigurator configurator)
            where T : class
        {
            return BusTestHarness.Handled<T>(configurator);
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
            return BusTestHarness.HandledByConsumer<T>(configurator);
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
            return BusTestHarness.Handled(configurator, filter);
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
            return BusTestHarness.Handled<T>(configurator, expectedCount);
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
            return BusTestHarness.Handler(configurator, handler);
        }

        protected virtual void ConnectObservers(IBus bus)
        {
        }
    }
}
