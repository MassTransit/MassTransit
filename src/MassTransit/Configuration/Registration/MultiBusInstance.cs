namespace MassTransit.Registration
{
    using System;
    using Configuration;
    using GreenPipes;
    using Riders;


    public class MultiBusInstance<TBus> :
        IBusInstance<TBus>
        where TBus : IBus
    {
        readonly IBusInstance _instance;

        public MultiBusInstance(TBus bus, IBusInstance instance)
        {
            _instance = instance;
            BusInstance = bus;
        }

        public Type InstanceType => typeof(TBus);
        public IBus Bus => BusInstance;
        public IBusControl BusControl => _instance.BusControl;
        public IHostConfiguration HostConfiguration => _instance.HostConfiguration;

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            return _instance.GetRider<TRider>();
        }

        public TBus BusInstance { get; }

        public ConnectHandle ConnectRider(IRider rider)
        {
            return _instance.ConnectRider(rider);
        }
    }
}
