namespace MassTransit.DependencyInjection.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using MassTransit.Testing.Implementations;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;


    public class ContainerTestHarness :
        ITestHarness,
        IAsyncDisposable
    {
        readonly Lazy<BusTestConsumeObserver> _consumed;
        readonly List<ConnectHandle> _handles;
        readonly Lazy<AsyncInactivityObserver> _inactivityObserver;
        readonly IServiceProvider _provider;
        readonly Lazy<BusTestPublishObserver> _published;
        readonly Lazy<BusTestReceiveObserver> _received;
        readonly Lazy<IServiceScope> _scope;
        readonly Lazy<BusTestSendObserver> _sent;
        CancellationToken _cancellationToken;
        CancellationTokenSource _cancellationTokenSource;
        Task<bool> _cancelledTask;
        bool _disposing;
        IEnumerable<IHostedService> _hostedServices;
        CancellationTokenRegistration _registration;

        public ContainerTestHarness(IServiceProvider provider, IOptions<TestHarnessOptions> options)
        {
            _provider = provider;

            _handles = new List<ConnectHandle>(5);

            TestTimeout = options.Value.TestTimeout;
            TestInactivityTimeout = options.Value.TestInactivityTimeout;

            _inactivityObserver = new Lazy<AsyncInactivityObserver>(() => new AsyncInactivityObserver(TestInactivityTimeout, CancellationToken));

            _consumed = new Lazy<BusTestConsumeObserver>(() => new BusTestConsumeObserver(TestTimeout, InactivityToken));
            _published = new Lazy<BusTestPublishObserver>(() => new BusTestPublishObserver(TestTimeout, TestInactivityTimeout, InactivityToken));
            _received = new Lazy<BusTestReceiveObserver>(() => new BusTestReceiveObserver(TestInactivityTimeout));
            _sent = new Lazy<BusTestSendObserver>(() => new BusTestSendObserver(TestTimeout, TestInactivityTimeout, InactivityToken));

            _scope = new Lazy<IServiceScope>(() => _provider.CreateScope());

            provider.GetService<TestActivityListener>();
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposing)
                return;

            _disposing = true;

            if (_hostedServices != null)
                await Task.WhenAll(_hostedServices.Select(x => x.StopAsync(CancellationToken)));

            if (_scope.IsValueCreated)
            {
                switch (_scope.Value)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync();
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }

            _registration.Dispose();

            _handles.ForEach(handle => handle.Disconnect());
            _handles.Clear();

            if (_consumed.IsValueCreated)
                _consumed.Value.Dispose();
            if (_published.IsValueCreated)
                _published.Value.Dispose();
            if (_received.IsValueCreated)
                _received.Value.Dispose();
            if (_sent.IsValueCreated)
                _sent.Value.Dispose();
        }

        public Task InactivityTask => _inactivityObserver.Value.InactivityTask;

        public IReceivedMessageList Consumed => _consumed.Value.Messages;
        public IPublishedMessageList Published => _published.Value.Messages;
        public ISentMessageList Sent => _sent.Value.Messages;

        public IServiceScope Scope => _scope.Value;
        public IServiceProvider Provider => _provider;

        public IEndpointNameFormatter EndpointNameFormatter => _provider.GetService<IEndpointNameFormatter>() ?? DefaultEndpointNameFormatter.Instance;

        public IBus Bus => _provider.GetRequiredService<IBus>();

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void ForceInactive()
        {
            _inactivityObserver.Value.ForceInactive();
        }

        public IConsumerTestHarness<T> GetConsumerHarness<T>()
            where T : class, IConsumer
        {
            return _provider.GetRequiredService<IConsumerTestHarness<T>>();
        }

        public ISagaTestHarness<T> GetSagaHarness<T>()
            where T : class, ISaga
        {
            return _provider.GetRequiredService<ISagaTestHarness<T>>();
        }

        public ISagaStateMachineTestHarness<TStateMachine, T> GetSagaStateMachineHarness<TStateMachine, T>()
            where TStateMachine : class, SagaStateMachine<T>
            where T : class, SagaStateMachineInstance
        {
            return _provider.GetRequiredService<ISagaStateMachineTestHarness<TStateMachine, T>>();
        }

        public IRequestClient<T> GetRequestClient<T>()
            where T : class
        {
            return _scope.Value.ServiceProvider.GetRequiredService<IRequestClient<T>>();
        }

        public Task<ISendEndpoint> GetConsumerEndpoint<T>()
            where T : class, IConsumer
        {
            var provider = _scope.Value.ServiceProvider.GetRequiredService<ISendEndpointProvider>();

            return provider.GetSendEndpoint(GetConsumerAddress<T>());
        }

        public Task<ISendEndpoint> GetHandlerEndpoint<T>()
            where T : class
        {
            return GetConsumerEndpoint<MessageHandlerConsumer<T>>();
        }

        public Uri GetConsumerAddress<T>()
            where T : class, IConsumer
        {
            return new Uri($"queue:{EndpointNameFormatter.Consumer<T>()}");
        }

        public Uri GetHandlerAddress<T>()
            where T : class
        {
            return GetConsumerAddress<MessageHandlerConsumer<T>>();
        }

        public Task<ISendEndpoint> GetSagaEndpoint<T>()
            where T : class, ISaga
        {
            var provider = _scope.Value.ServiceProvider.GetRequiredService<ISendEndpointProvider>();

            return provider.GetSendEndpoint(GetSagaAddress<T>());
        }

        public Uri GetSagaAddress<T>()
            where T : class, ISaga
        {
            return new Uri($"queue:{EndpointNameFormatter.Saga<T>()}");
        }

        public Task<ISendEndpoint> GetExecuteActivityEndpoint<T, TArguments>()
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var provider = _scope.Value.ServiceProvider.GetRequiredService<ISendEndpointProvider>();

            return provider.GetSendEndpoint(GetExecuteActivityAddress<T, TArguments>());
        }

        public Uri GetExecuteActivityAddress<T, TArguments>()
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return new Uri($"queue:{EndpointNameFormatter.ExecuteActivity<T, TArguments>()}");
        }

        public async Task Start()
        {
            _hostedServices = _provider.GetServices<IHostedService>().ToArray();
            if (_hostedServices == null)
                throw new ConfigurationException("The MassTransit hosted service was not found.");

            foreach (var service in _hostedServices)
                await service.StartAsync(CancellationToken).ConfigureAwait(false);
        }

        public TimeSpan TestTimeout { get; set; }
        public TimeSpan TestInactivityTimeout { get; set; }

        /// <summary>
        /// CancellationToken that is cancelled when the test inactivity timeout has elapsed with no bus activity
        /// </summary>
        public CancellationToken InactivityToken => _inactivityObserver.Value.InactivityToken;

        /// <summary>
        /// CancellationToken that is canceled when the test is being aborted
        /// </summary>
        public CancellationToken CancellationToken
        {
            get
            {
                if (_cancellationToken == CancellationToken.None)
                {
                    _cancellationTokenSource = new CancellationTokenSource((int)TestTimeout.TotalMilliseconds);
                    _cancellationToken = _cancellationTokenSource.Token;

                    var source = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    _cancelledTask = source.Task;

                    _registration = _cancellationToken.Register(() => source.TrySetCanceled());
                }

                return _cancellationToken;
            }
        }

        public TaskCompletionSource<T> GetTask<T>()
        {
            var source = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (CancellationToken.IsCancellationRequested)
                source.TrySetCanceled(CancellationToken);
            else
                _cancelledTask.ContinueWith(x => source.TrySetCanceled(), TaskContinuationOptions.OnlyOnCanceled);

            return source;
        }

        public void PostCreate(IBus bus)
        {
            _handles.Add(bus.ConnectReceiveEndpointObserver(new TestReceiveEndpointObserver(_published.Value)));

            _handles.Add(_consumed.Value.ConnectInactivityObserver(_inactivityObserver.Value));
            _handles.Add(_published.Value.ConnectInactivityObserver(_inactivityObserver.Value));
            _handles.Add(_received.Value.ConnectInactivityObserver(_inactivityObserver.Value));
            _handles.Add(_sent.Value.ConnectInactivityObserver(_inactivityObserver.Value));

            _handles.Add(bus.ConnectConsumeObserver(_consumed.Value));
            _handles.Add(bus.ConnectPublishObserver(_published.Value));
            _handles.Add(bus.ConnectReceiveObserver(_received.Value));
            _handles.Add(bus.ConnectSendObserver(_sent.Value));
        }

        public void PostStart(IBus bus)
        {
            _ = _received.Value.RestartTimer();
            _ = _published.Value.RestartTimer();
            _ = _sent.Value.RestartTimer();
        }
    }
}
