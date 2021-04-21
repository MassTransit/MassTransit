namespace MassTransit.GrpcTransport.Testing
{
    using System;
    using MassTransit.Configuration;
    using Registration;
    using Riders;


    public class GrpcTestHarnessBusInstance :
        IBusInstance
    {
        readonly IBusRegistrationContext _busRegistrationContext;

        public GrpcTestHarnessBusInstance(GrpcTestHarness testHarness, IBusRegistrationContext busRegistrationContext)
        {
            _busRegistrationContext = busRegistrationContext;
            Harness = testHarness;
        }

        public GrpcTestHarness Harness { get; }

        public Type InstanceType => typeof(GrpcTestHarness);
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
