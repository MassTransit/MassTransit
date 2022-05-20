namespace MassTransit.Transports
{
    using System;
    using Configuration;


    public class MultiBusInstance<TBus> :
        IBusInstance<TBus>
        where TBus : IBus
    {
        readonly TBus _bus;

        public MultiBusInstance(TBus bus, IBusInstance instance)
        {
            BusInstance = instance;
            _bus = bus;
        }

        public string Name { get; } = FormatBusName();
        public Type InstanceType => typeof(TBus);
        public IBus Bus => _bus;
        public IBusInstance BusInstance { get; }
        public IBusControl BusControl => BusInstance.BusControl;
        public IHostConfiguration HostConfiguration => BusInstance.HostConfiguration;

        TBus IBusInstance<TBus>.Bus => _bus;

        public void Connect<TRider>(IRiderControl riderControl)
            where TRider : IRider
        {
            BusInstance.Connect<TRider>(riderControl);
        }

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            return BusInstance.GetRider<TRider>();
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null)
        {
            return BusInstance.ConnectReceiveEndpoint(definition, endpointNameFormatter, configure);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName,
            Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null)
        {
            return BusInstance.ConnectReceiveEndpoint(queueName, configure);
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
