namespace MassTransit.Testing.Implementations
{
    using System;
    using Configuration;
    using Transports;


    public class InMemoryTestHarnessBusInstance :
        IBusInstance
    {
        readonly IBusRegistrationContext _busRegistrationContext;

        public InMemoryTestHarnessBusInstance(InMemoryTestHarness testHarness, IBusRegistrationContext busRegistrationContext)
        {
            _busRegistrationContext = busRegistrationContext;
            Harness = testHarness;
        }

        public InMemoryTestHarness Harness { get; }

        public string Name => "masstransit-bus";
        public Type InstanceType => typeof(IBus);
        public IBus Bus => Harness.Bus;
        public IBusControl BusControl => Harness.BusControl;
        public IHostConfiguration HostConfiguration => Harness.HostConfiguration;

        public void Connect<TRider>(IRiderControl riderControl)
            where TRider : IRider
        {
            throw new NotSupportedException();
        }

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            throw new NotSupportedException();
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null)
        {
            return BusControl.ConnectReceiveEndpoint(definition, endpointNameFormatter, configurator =>
            {
                _busRegistrationContext.GetConfigureReceiveEndpoints().Configure(definition.GetEndpointName(endpointNameFormatter), configurator);

                configure?.Invoke(_busRegistrationContext, configurator);
            });
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName,
            Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null)
        {
            return BusControl.ConnectReceiveEndpoint(queueName, configurator =>
            {
                _busRegistrationContext.GetConfigureReceiveEndpoints().Configure(queueName, configurator);

                configure?.Invoke(_busRegistrationContext, configurator);
            });
        }
    }
}
