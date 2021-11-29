namespace MassTransit.Transports
{
    using System;
    using Configuration;


    public class MultiBusInstance<TBus> :
        IBusInstance<TBus>
        where TBus : IBus
    {
        readonly IBusInstance _instance;

        public MultiBusInstance(TBus bus, IBusInstance instance)
        {
            _instance = instance;
            BusInstance = bus;
        }

        public string Name { get; } = FormatBusName();
        public Type InstanceType => typeof(TBus);
        public IBus Bus => BusInstance;
        public IBusControl BusControl => _instance.BusControl;
        public IHostConfiguration HostConfiguration => _instance.HostConfiguration;

        public void Connect<TRider>(IRiderControl riderControl)
            where TRider : IRider
        {
            _instance.Connect<TRider>(riderControl);
        }

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            return _instance.GetRider<TRider>();
        }

        public TBus BusInstance { get; }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null)
        {
            return _instance.ConnectReceiveEndpoint(definition, endpointNameFormatter, configure);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName,
            Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null)
        {
            return _instance.ConnectReceiveEndpoint(queueName, configure);
        }

        static string FormatBusName()
        {
            var name = typeof(TBus).Name;
            if (name.Length >= 2 && name[0] == 'I' && char.IsUpper(name[1]))
                name = name.Substring(1);

            return $"masstransit-{KebabCaseEndpointNameFormatter.Instance.SanitizeName(name)}";
        }
    }
}
