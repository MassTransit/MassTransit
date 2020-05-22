namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using Registration;


    public class ServiceBusBusInstance :
        IBusInstance
    {
        public ServiceBusBusInstance(IBusControl busControl, IServiceBusHostConfiguration hostConfiguration)
        {
            BusControl = busControl;
            BusConnector = new ServiceBusConnector(hostConfiguration);
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }

        public IBusConnector BusConnector { get; }
    }
}
