namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using InMemoryTransport.Configuration;


    public class InMemoryTestHarness :
        BusTestHarness
    {
        readonly InMemoryBusConfiguration _busConfiguration;
        readonly string _inputQueueName;
        readonly IEnumerable<IBusInstanceSpecification> _specifications;

        public InMemoryTestHarness(string virtualHost = null)
            : this(virtualHost, Enumerable.Empty<IBusInstanceSpecification>())
        {
        }

        public InMemoryTestHarness(string virtualHost, IEnumerable<IBusInstanceSpecification> specifications)
        {
            BaseAddress = new Uri("loopback://localhost/");
            if (!string.IsNullOrWhiteSpace(virtualHost))
                BaseAddress = new Uri(BaseAddress, virtualHost.Trim('/') + '/');

            _inputQueueName = "input_queue";
            _busConfiguration = new InMemoryBusConfiguration(new InMemoryTopologyConfiguration(InMemoryBus.MessageTopology), BaseAddress);
            _specifications = specifications;

            InputQueueAddress = new Uri(BaseAddress, _inputQueueName);
        }

        public Uri BaseAddress { get; }

        public override Uri InputQueueAddress { get; }
        public override string InputQueueName => _inputQueueName;

        internal IHostConfiguration HostConfiguration => _busConfiguration?.HostConfiguration;

        public event Action<IInMemoryBusFactoryConfigurator> OnConfigureInMemoryBus;
        public event Action<IInMemoryReceiveEndpointConfigurator> OnConfigureInMemoryReceiveEndpoint;
        public event Action<IInMemoryBusFactoryConfigurator> OnInMemoryBusConfigured;

        protected virtual void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            OnConfigureInMemoryBus?.Invoke(configurator);
        }

        protected virtual void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            OnConfigureInMemoryReceiveEndpoint?.Invoke(configurator);
        }

        protected virtual void InMemoryBusConfigured(IInMemoryBusFactoryConfigurator configurator)
        {
            OnInMemoryBusConfigured?.Invoke(configurator);
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
            var configurator = new InMemoryBusFactoryConfigurator(_busConfiguration);

            ConfigureBus(configurator);

            ConfigureInMemoryBus(configurator);

            configurator.ReceiveEndpoint(InputQueueName, e =>
            {
                ConfigureReceiveEndpoint(e);

                ConfigureInMemoryReceiveEndpoint(e);
            });

            BusConfigured(configurator);

            InMemoryBusConfigured(configurator);

            return configurator.Build(_busConfiguration, _specifications ?? Enumerable.Empty<ISpecification>());
        }
    }
}
