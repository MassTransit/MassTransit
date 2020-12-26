namespace MassTransit.Testing
{
    using System;
    using Configuration;
    using Registration;
    using Riders;


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

        public Type InstanceType => typeof(InMemoryTestHarness);
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
                configure?.Invoke(_busRegistrationContext, configurator);
            });
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName,
            Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null)
        {
            return BusControl.ConnectReceiveEndpoint(queueName, configurator =>
            {
                configure?.Invoke(_busRegistrationContext, configurator);
            });
        }
    }
}
