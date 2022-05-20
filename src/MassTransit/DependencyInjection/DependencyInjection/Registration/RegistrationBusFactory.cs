namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Transports;


    public class RegistrationBusFactory :
        IRegistrationBusFactory
    {
        readonly Func<IBusRegistrationContext, IBusControl> _configure;

        public RegistrationBusFactory(Func<IBusRegistrationContext, IBusControl> configure)
        {
            _configure = configure ?? throw new ArgumentNullException(nameof(configure));
        }

        public IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName)
        {
            LogContext.ConfigureCurrentLogContextIfNull(context);

            var busControl = _configure(context);

            return new DefaultBusInstance(busControl, context);
        }


        class DefaultBusInstance :
            IBusInstance
        {
            const string RiderExceptionMessage =
                "Riders could be only used with Microsoft DI or Autofac using 'SetBusFactory' method (UsingTransport extensions).";

            readonly IBusRegistrationContext _busRegistrationContext;

            public DefaultBusInstance(IBusControl busControl, IBusRegistrationContext busRegistrationContext)
            {
                _busRegistrationContext = busRegistrationContext;
                BusControl = busControl;
            }

            public string Name => "masstransit-bus";
            public Type InstanceType => typeof(IBus);
            public IBus Bus => BusControl;
            public IBusControl BusControl { get; }

            public IHostConfiguration HostConfiguration => default;

            public void Connect<TRider>(IRiderControl riderControl)
                where TRider : IRider
            {
                throw new ConfigurationException(RiderExceptionMessage);
            }

            public TRider GetRider<TRider>()
                where TRider : IRider
            {
                throw new ConfigurationException(RiderExceptionMessage);
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
}
