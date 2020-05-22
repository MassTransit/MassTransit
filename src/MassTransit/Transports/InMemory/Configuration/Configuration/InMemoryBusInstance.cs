namespace MassTransit.Transports.InMemory.Configuration
{
    using System;
    using Registration;


    public class InMemoryBusInstance :
        IBusInstance
    {
        public InMemoryBusInstance(IBusControl busControl, IInMemoryHostConfiguration hostConfiguration)
        {
            BusControl = busControl;
            BusConnector = new InMemoryBusConnector(hostConfiguration);
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }
        public IBusConnector BusConnector { get; }
    }
}
