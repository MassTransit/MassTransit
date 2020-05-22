namespace MassTransit.Registration
{
    using System;


    public interface IBusInstance
    {
        Type InstanceType { get; }

        IBus Bus { get; }
        IBusControl BusControl { get; }
        IBusConnector BusConnector { get; }
    }


    public interface IBusInstance<out TBus> :
        IBusInstance
        where TBus : IBus
    {
        TBus BusInstance { get; }
    }
}
