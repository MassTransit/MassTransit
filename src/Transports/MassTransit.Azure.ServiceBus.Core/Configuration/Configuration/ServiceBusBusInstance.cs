namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using Registration;


    public class ServiceBusBusInstance :
        IBusInstance
    {
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public ServiceBusBusInstance(IBusControl busControl, IServiceBusHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            BusControl = busControl;
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }
    }
}
