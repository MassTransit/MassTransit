namespace MassTransit.Transports.InMemory.Configuration
{
    using System;
    using Registration;


    public class InMemoryBusInstance :
        IBusInstance
    {
        readonly IInMemoryHostConfiguration _hostConfiguration;

        public InMemoryBusInstance(IBusControl busControl, IInMemoryHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            BusControl = busControl;
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }
    }
}
