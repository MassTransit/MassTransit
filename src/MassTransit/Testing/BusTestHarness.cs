namespace MassTransit.Testing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Observers;
    using Util;


    /// <summary>
    /// A bus text fixture includes a single bus instance with one or more receiving endpoints.
    /// </summary>
    public abstract class BusTestHarness :
        AsyncTestHarness
    {
        IBusControl _bus;
        ConnectHandle _busConsumeObserver;
        BusHandle _busHandle;
        ConnectHandle _busPublishObserver;
        ConnectHandle _busSendObserver;
        BusTestConsumeObserver _consumed;
        ConnectHandle _inputQueueSendObserver;
        BusTestPublishObserver _published;
        ConnectHandle _receiveEndpointObserver;
        TestSendObserver _sent;

        public IBus Bus => _bus;
        public IBusControl BusControl => _bus;

        /// <summary>
        /// The address of the default bus endpoint, used as the SourceAddress for requests and published messages
        /// </summary>
        public Uri BusAddress => _bus.Address;

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

        public ISentMessageList Sent => _sent.Messages;
        public IReceivedMessageList Consumed => _consumed.Messages;
        public IPublishedMessageList Published => _published.Messages;

        protected abstract IBusControl CreateBus();

        public virtual IRequestClient<TRequest, TResponse> CreateRequestClient<TRequest, TResponse>()
            where TRequest : class
            where TResponse : class
        {
            return CreateRequestClient<TRequest, TResponse>(InputQueueAddress);
        }

        public virtual IRequestClient<TRequest> CreateRequestClient<TRequest>()
            where TRequest : class
        {
            return CreateRequestClient<TRequest>(InputQueueAddress);
        }

        public virtual IRequestClient<TRequest, TResponse> CreateRequestClient<TRequest, TResponse>(Uri destinationAddress)
            where TRequest : class
            where TResponse : class
        {
            return Bus.CreateRequestClient<TRequest, TResponse>(destinationAddress, TestTimeout);
        }

        public virtual IRequestClient<TRequest> CreateRequestClient<TRequest>(Uri destinationAddress)
            where TRequest : class
        {
            return Bus.CreateRequestClient<TRequest>(destinationAddress, TestTimeout);
        }

        protected virtual void ConnectObservers(IBus bus)
        {
            _receiveEndpointObserver = bus.ConnectReceiveEndpointObserver(new TestReceiveEndpointObserver(_published));

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

        public event Action<BusTestHarness> PreCreateBus;
        public event Action<IReceiveEndpointConfigurator> OnConfigureReceiveEndpoint;
        public event Action<IBusFactoryConfigurator> OnConfigureBus;
        public event Action<IBus> OnConnectObservers;

        public virtual async Task Start(CancellationToken cancellationToken = default)
        {
            _sent = new TestSendObserver(TestTimeout);
            _consumed = new BusTestConsumeObserver(TestTimeout);
            _published = new BusTestPublishObserver(TestTimeout);

            PreCreateBus?.Invoke(this);

            _bus = CreateBus();

            ConnectObservers(_bus);

            _busHandle = await _bus.StartAsync(cancellationToken).ConfigureAwait(false);

            BusSendEndpoint = await GetSendEndpoint(_bus.Address).ConfigureAwait(false);

            InputQueueSendEndpoint = await GetSendEndpoint(InputQueueAddress).ConfigureAwait(false);

            _inputQueueSendObserver = InputQueueSendEndpoint.ConnectSendObserver(_sent);

            _busConsumeObserver = _bus.ConnectConsumeObserver(_consumed);

            _busPublishObserver = _bus.ConnectPublishObserver(_published);

            _busSendObserver = _bus.ConnectSendObserver(_sent);
        }

        public virtual async Task Stop()
        {
            try
            {
                _receiveEndpointObserver?.Disconnect();
                _receiveEndpointObserver = null;

                _busSendObserver?.Disconnect();
                _busSendObserver = null;

                _inputQueueSendObserver?.Disconnect();
                _inputQueueSendObserver = null;

                _busPublishObserver?.Disconnect();
                _busPublishObserver = null;

                _busConsumeObserver?.Disconnect();
                _busConsumeObserver = null;

                using (var tokenSource = new CancellationTokenSource(TestTimeout))
                {
                    await (_busHandle?.StopAsync(tokenSource.Token) ?? TaskUtil.Completed).ConfigureAwait(false);
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
                _bus = null;
            }
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return await _bus.GetSendEndpoint(address).ConfigureAwait(false);
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
            var source = TaskUtil.GetTask<ConsumeContext<T>>();

            ConnectHandle handler = null;
            handler = Bus.ConnectHandler<T>(async context =>
            {
                source.SetResult(context);

                handler.Disconnect();
            });

            TestCancelledTask.ContinueWith(x =>
            {
                source.TrySetCanceled();

                handler.Disconnect();
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
            var source = TaskUtil.GetTask<ConsumeContext<T>>();

            ConnectHandle handler = null;
            handler = Bus.ConnectHandler<T>(async context =>
            {
                if (filter(context))
                {
                    source.SetResult(context);

                    handler.Disconnect();
                }
            });

            TestCancelledTask.ContinueWith(x =>
            {
                source.TrySetCanceled();

                handler.Disconnect();
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
                int value = Interlocked.Increment(ref count);
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

                return TaskUtil.Completed;
            }
        }
    }
}
