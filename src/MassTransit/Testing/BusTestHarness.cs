namespace MassTransit.Testing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Implementations;
    using Util;


    /// <summary>
    /// A bus text fixture includes a single bus instance with one or more receiving endpoints.
    /// </summary>
    public abstract class BusTestHarness :
        AsyncTestHarness,
        IBaseTestHarness
    {
        BusHandle _busHandle;
        BusTestConsumeObserver _consumed;
        BusTestPublishObserver _published;
        BusTestReceiveObserver _received;
        BusTestSendObserver _sent;

        public IBusControl BusControl { get; private set; }

        /// <summary>
        /// The address of the default bus endpoint, used as the SourceAddress for requests and published messages
        /// </summary>
        public Uri BusAddress => BusControl.Address;

        /// <summary>
        /// The name of the input queue (for the default receive endpoint)
        /// </summary>
        public abstract string InputQueueName { get; }

        /// <summary>
        /// The address of the input queue receive endpoint
        /// </summary>
        public abstract Uri InputQueueAddress { get; }

        /// <summary>
        /// The send endpoint for the default bus endpoint
        /// </summary>
        public ISendEndpoint BusSendEndpoint { get; private set; }

        /// <summary>
        /// The send endpoint for the input queue receive endpoint
        /// </summary>
        public ISendEndpoint InputQueueSendEndpoint { get; private set; }

        public IBus Bus => BusControl;

        public ISentMessageList Sent => _sent.Messages;
        public CancellationToken CancellationToken => TestCancellationToken;
        public IReceivedMessageList Consumed => _consumed.Messages;
        public IPublishedMessageList Published => _published.Messages;

        protected abstract IBusControl CreateBus();

        public virtual IRequestClient<TRequest> CreateRequestClient<TRequest>()
            where TRequest : class
        {
            return CreateRequestClient<TRequest>(InputQueueAddress);
        }

        public virtual IRequestClient<TRequest> CreateRequestClient<TRequest>(Uri destinationAddress)
            where TRequest : class
        {
            return Bus.CreateRequestClient<TRequest>(destinationAddress, TestTimeout);
        }

        protected virtual void ConnectObservers(IBus bus)
        {
            bus.ConnectReceiveEndpointObserver(new TestReceiveEndpointObserver(_published));

            OnConnectObservers?.Invoke(bus);
        }

        protected virtual void ConfigureBus(IBusFactoryConfigurator configurator)
        {
            OnConfigureBus?.Invoke(configurator);
        }

        protected virtual void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            OnConfigureReceiveEndpoint?.Invoke(configurator);
        }

        protected virtual void BusConfigured(IBusFactoryConfigurator configurator)
        {
            OnBusConfigured?.Invoke(configurator);
        }

        public event Action<BusTestHarness> PreCreateBus;
        public event Action<IReceiveEndpointConfigurator> OnConfigureReceiveEndpoint;
        public event Action<IBusFactoryConfigurator> OnConfigureBus;
        public event Action<IBusFactoryConfigurator> OnBusConfigured;
        public event Action<IBus> OnConnectObservers;

        public virtual async Task Start(CancellationToken cancellationToken = default)
        {
            if (!cancellationToken.CanBeCanceled)
                cancellationToken = TestCancellationToken;

            _received = new BusTestReceiveObserver(TestInactivityTimeout);
            _received.ConnectInactivityObserver(InactivityObserver);

            _consumed = new BusTestConsumeObserver(TestTimeout, InactivityToken);
            _consumed.ConnectInactivityObserver(InactivityObserver);

            _published = new BusTestPublishObserver(TestTimeout, TestInactivityTimeout, InactivityToken);
            _published.ConnectInactivityObserver(InactivityObserver);

            _sent = new BusTestSendObserver(TestTimeout, TestInactivityTimeout, InactivityToken);
            _sent.ConnectInactivityObserver(InactivityObserver);

            PreCreateBus?.Invoke(this);

            BusControl = CreateBus();

            ConnectObservers(BusControl);

            _busHandle = await BusControl.StartAsync(cancellationToken).ConfigureAwait(false);

            BusSendEndpoint = await GetSendEndpoint(BusControl.Address).ConfigureAwait(false);

            InputQueueSendEndpoint = await GetSendEndpoint(InputQueueAddress).ConfigureAwait(false);

            InputQueueSendEndpoint.ConnectSendObserver(_sent);

            BusControl.ConnectConsumeObserver(_consumed);
            BusControl.ConnectPublishObserver(_published);
            BusControl.ConnectReceiveObserver(_received);
            BusControl.ConnectSendObserver(_sent);
        }

        public virtual async Task Stop()
        {
            try
            {
                if (_busHandle != null)
                {
                    using var tokenSource = new CancellationTokenSource(TestTimeout);

                    await _busHandle.StopAsync(tokenSource.Token).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Stop bus faulted");
                throw;
            }
            finally
            {
                _busHandle = null;
                BusControl = null;
            }
        }

        public virtual async Task Clean()
        {
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return await BusControl.GetSendEndpoint(address).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes a message handler to the bus, which is disconnected after the message
        /// is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns>An awaitable task completed when the message is received</returns>
        public Task<ConsumeContext<T>> SubscribeHandler<T>()
            where T : class
        {
            TaskCompletionSource<ConsumeContext<T>> source = TaskUtil.GetTask<ConsumeContext<T>>();

            ConnectHandle handler = null;
            handler = Bus.ConnectHandler<T>(async context =>
            {
                handler.Disconnect();

                source.SetResult(context);
            });

            TestCancelledTask.ContinueWith(x =>
            {
                handler.Disconnect();

                source.TrySetCanceled();
            }, TaskContinuationOptions.OnlyOnCanceled);

            return source.Task;
        }

        /// <summary>
        /// Subscribes a message handler to the bus, which is disconnected after the message
        /// is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="filter">A filter that only completes the task if filter is true</param>
        /// <returns>An awaitable task completed when the message is received</returns>
        public Task<ConsumeContext<T>> SubscribeHandler<T>(Func<ConsumeContext<T>, bool> filter)
            where T : class
        {
            TaskCompletionSource<ConsumeContext<T>> source = TaskUtil.GetTask<ConsumeContext<T>>();

            ConnectHandle handler = null;
            handler = Bus.ConnectHandler<T>(async context =>
            {
                if (filter(context))
                {
                    handler.Disconnect();

                    source.SetResult(context);
                }
            });

            TestCancelledTask.ContinueWith(x =>
            {
                handler.Disconnect();

                source.TrySetCanceled();
            }, TaskContinuationOptions.OnlyOnCanceled);

            return source.Task;
        }

        /// <summary>
        /// Registers a handler on the receive endpoint that is cancelled when the test is canceled
        /// and completed when the message is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The endpoint configurator</param>
        /// <returns></returns>
        public Task<ConsumeContext<T>> Handled<T>(IReceiveEndpointConfigurator configurator)
            where T : class
        {
            TaskCompletionSource<ConsumeContext<T>> source = GetTask<ConsumeContext<T>>();

            configurator.Handler<T>(async context => source.TrySetResult(context));

            return source.Task;
        }

        /// <summary>
        /// Registers a handler on the receive endpoint that is cancelled when the test is canceled
        /// and completed when the message is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The endpoint configurator</param>
        /// <param name="filter">Filter the messages based on the handled consume context</param>
        /// <returns></returns>
        public Task<ConsumeContext<T>> Handled<T>(IReceiveEndpointConfigurator configurator, Func<ConsumeContext<T>, bool> filter)
            where T : class
        {
            TaskCompletionSource<ConsumeContext<T>> source = GetTask<ConsumeContext<T>>();

            configurator.Handler<T>(async context =>
            {
                if (filter(context))
                    source.TrySetResult(context);
            });

            return source.Task;
        }

        /// <summary>
        /// Registers a handler on the receive endpoint that is cancelled when the test is canceled
        /// and completed when the message is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The endpoint configurator</param>
        /// <param name="expectedCount">The expected number of messages</param>
        /// <returns></returns>
        public Task<ConsumeContext<T>> Handled<T>(IReceiveEndpointConfigurator configurator, int expectedCount)
            where T : class
        {
            TaskCompletionSource<ConsumeContext<T>> source = GetTask<ConsumeContext<T>>();

            var count = 0;
            configurator.Handler<T>(async context =>
            {
                var value = Interlocked.Increment(ref count);
                if (value == expectedCount)
                    source.TrySetResult(context);
            });

            return source.Task;
        }

        /// <summary>
        /// Registers a handler on the receive endpoint that is completed after the specified handler is
        /// executed and canceled if the test is canceled.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public Task<ConsumeContext<T>> Handler<T>(IReceiveEndpointConfigurator configurator, MessageHandler<T> handler)
            where T : class
        {
            TaskCompletionSource<ConsumeContext<T>> source = GetTask<ConsumeContext<T>>();

            configurator.Handler<T>(async context =>
            {
                await handler(context).ConfigureAwait(false);
                source.TrySetResult(context);
            });

            return source.Task;
        }

        /// <summary>
        /// Registers a consumer on the receive endpoint that is cancelled when the test is canceled
        /// and completed when the message is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The endpoint configurator</param>
        /// <returns></returns>
        public Task<ConsumeContext<T>> HandledByConsumer<T>(IReceiveEndpointConfigurator configurator)
            where T : class
        {
            TaskCompletionSource<ConsumeContext<T>> source = GetTask<ConsumeContext<T>>();

            configurator.Consumer(() => new Consumer<T>(source));

            return source.Task;
        }


        class Consumer<T> :
            IConsumer<T>
            where T : class
        {
            readonly TaskCompletionSource<ConsumeContext<T>> _source;

            public Consumer(TaskCompletionSource<ConsumeContext<T>> source)
            {
                _source = source;
            }

            public Task Consume(ConsumeContext<T> context)
            {
                _source.TrySetResult(context);

                return Task.CompletedTask;
            }
        }
    }
}
