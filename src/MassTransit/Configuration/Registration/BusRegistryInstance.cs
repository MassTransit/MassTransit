namespace MassTransit.Registration
{
    using System;


    public class BusRegistryInstance<TBus> :
        IBusRegistryInstance<TBus>
        where TBus : IBus
    {
        public BusRegistryInstance(TBus bus, Bind<TBus, IBusControl> busControl)
        {
            BusInstance = bus;
            BusControl = busControl.Value;
        }

        public Type InstanceType => typeof(TBus);
        public IBus Bus => BusInstance;
        public IBusControl BusControl { get; }
        public TBus BusInstance { get; }
    }


    public class BusRegistryInstance :
        IBusRegistryInstance
    {
        public BusRegistryInstance(IBusControl busControl)
        {
            BusControl = busControl;
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }
    }
}
