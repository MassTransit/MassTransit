namespace MassTransit.Testing
{
    using System;
    using System.Threading.Tasks;


    public class InMemoryTestHarness :
        BusTestHarness
    {
        readonly string _inputQueueName;

        public InMemoryTestHarness(string virtualHost = null)
        {
            BaseAddress = new Uri("loopback://localhost/");
            if (!string.IsNullOrWhiteSpace(virtualHost))
                BaseAddress = new Uri(BaseAddress, virtualHost.Trim('/') + '/');

            _inputQueueName = "input_queue";
            InputQueueAddress = new Uri(BaseAddress, _inputQueueName);
        }

        public Uri BaseAddress { get; }

        public override Uri InputQueueAddress { get; }
        public override string InputQueueName => _inputQueueName;

        public event Action<IInMemoryBusFactoryConfigurator> OnConfigureInMemoryBus;
        public event Action<IInMemoryReceiveEndpointConfigurator> OnConfigureInMemoryReceiveEndpoint;

        protected virtual void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            OnConfigureInMemoryBus?.Invoke(configurator);
        }

        protected virtual void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            OnConfigureInMemoryReceiveEndpoint?.Invoke(configurator);
        }

        public virtual Task<IRequestClient<TRequest>> ConnectRequestClient<TRequest>()
            where TRequest : class
        {
            return ConnectRequestClient<TRequest>(InputQueueAddress);
        }

        public virtual Task<IRequestClient<TRequest>> ConnectRequestClient<TRequest>(Uri destinationAddress)
            where TRequest : class
        {
            return Task.FromResult(Bus.CreateRequestClient<TRequest>(destinationAddress, TestTimeout));
        }

        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingInMemory(BaseAddress, x =>
            {
                ConfigureBus(x);

                x.Host(BaseAddress);

                ConfigureInMemoryBus(x);

                x.ReceiveEndpoint(InputQueueName, configurator =>
                {
                    ConfigureReceiveEndpoint(configurator);

                    ConfigureInMemoryReceiveEndpoint(configurator);
                });
            });
        }
    }
}
