namespace MassTransit.Registration
{
    using System;


    public interface IBusRegistryInstance
    {
        Type InstanceType { get; }

        IBus Bus { get; }
        IBusControl BusControl { get; }
    }


    public interface IBusRegistryInstance<out TBus> :
        IBusRegistryInstance
        where TBus : IBus
    {
        TBus BusInstance { get; }
    }
}
