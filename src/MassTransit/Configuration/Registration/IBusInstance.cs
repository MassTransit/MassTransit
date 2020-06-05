namespace MassTransit.Registration
{
    using System;
    using Configuration;
    using Riders;


    public interface IBusInstance :
        IRiderConnector
    {
        Type InstanceType { get; }

        IBus Bus { get; }
        IBusControl BusControl { get; }

        IHostConfiguration HostConfiguration { get; }

        TRider GetRider<TRider>()
            where TRider : IRider;
    }


    public interface IBusInstance<out TBus> :
        IBusInstance
        where TBus : IBus
    {
        TBus BusInstance { get; }
    }
}
