namespace MassTransit.Transports
{
    using System;
    using Configuration;


    public interface IBusInstance :
        IReceiveEndpointConnector
    {
        string Name { get; }
        Type InstanceType { get; }

        IBus Bus { get; }
        IBusControl BusControl { get; }

        IHostConfiguration HostConfiguration { get; }

        void Connect<TRider>(IRiderControl riderControl)
            where TRider : IRider;

        TRider GetRider<TRider>()
            where TRider : IRider;
    }


    public interface IBusInstance<out TBus> :
        IBusInstance
        where TBus : IBus
    {
        new TBus Bus { get; }

        /// <summary>
        /// The original bus instance (since this is wrapped inside a multi-bus instance
        /// </summary>
        IBusInstance BusInstance { get; }
    }
}
