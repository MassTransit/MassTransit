namespace MassTransit.Transports
{
    using System;
    using System.Threading.Tasks;
    using Configuration;


    public class TransportBusInstance<TEndpointConfigurator> :
        IBusInstance,
        IReceiveEndpointConnector<TEndpointConfigurator>
        where TEndpointConfigurator : class, IReceiveEndpointConfigurator
    {
        readonly IHost<TEndpointConfigurator> _host;

        public TransportBusInstance(IBusControl busControl, IHost<TEndpointConfigurator> host, IHostConfiguration hostConfiguration, IBusRegistrationContext
            busRegistrationContext)
        {
            _host = host;
            RegistrationContext = busRegistrationContext;

            BusControl = busControl;
            HostConfiguration = hostConfiguration;
        }

        protected IBusRegistrationContext RegistrationContext { get; }

        public string Name => "masstransit-bus";
        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }

        public IHostConfiguration HostConfiguration { get; }

        public void Connect<TRider>(IRiderControl riderControl)
            where TRider : IRider
        {
            var name = GetRiderName<TRider>();
            _host.AddRider(name, riderControl);
        }

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            var name = GetRiderName<TRider>();
            return (TRider)_host.GetRider(name);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null)
        {
            return _host.ConnectReceiveEndpoint(definition, endpointNameFormatter, configurator =>
            {
                configure?.Invoke(RegistrationContext, configurator);
            });
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName,
            Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null)
        {
            return _host.ConnectReceiveEndpoint(queueName, configurator =>
            {
                configure?.Invoke(RegistrationContext, configurator);
            });
        }

        public async Task<bool> DisconnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter = null)
        {
            return await _host.DisconnectReceiveEndpoint(definition, endpointNameFormatter).ConfigureAwait(false);
        }

        public async Task<bool> DisconnectReceiveEndpoint(string queueName)
        {
            return await _host.DisconnectReceiveEndpoint(queueName).ConfigureAwait(false);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IBusRegistrationContext, TEndpointConfigurator> configure = null)
        {
            return _host.ConnectReceiveEndpoint(definition, endpointNameFormatter, configurator =>
            {
                RegistrationContext.GetConfigureReceiveEndpoints().Configure(definition.GetEndpointName(endpointNameFormatter), configurator);

                configure?.Invoke(RegistrationContext, configurator);
            });
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IBusRegistrationContext, TEndpointConfigurator> configure = null)
        {
            return _host.ConnectReceiveEndpoint(queueName, configurator =>
            {
                RegistrationContext.GetConfigureReceiveEndpoints().Configure(queueName, configurator);

                configure?.Invoke(RegistrationContext, configurator);
            });
        }

        static string GetRiderName<TRider>()
            where TRider : IRider
        {
            return TypeCache<TRider>.ShortName;
        }
    }
}
