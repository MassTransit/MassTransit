namespace MassTransit.Registration
{
    using System;
    using Configuration;


    public class TransportBusInstance :
        IBusInstance
    {
        public TransportBusInstance(IBusControl busControl, IHostConfiguration hostConfiguration)
        {
            BusControl = busControl;
            HostConfiguration = hostConfiguration;
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }

        public IHostConfiguration HostConfiguration { get; }
    }
}
