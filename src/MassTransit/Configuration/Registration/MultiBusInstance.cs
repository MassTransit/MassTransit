namespace MassTransit.Registration
{
    using System;
    using Configuration;
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

        public void Connect<TRider>(IRiderControl riderControl)
            where TRider : IRider
        {
            _instance.Connect<TRider>(riderControl);
        }

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            return _instance.GetRider<TRider>();
        }

        public TBus BusInstance { get; }
    }
}
