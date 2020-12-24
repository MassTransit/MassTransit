namespace MassTransit.Registration
{
    using System;
    using Configuration;
    using Riders;


    public class TransportBusInstance :
        IBusInstance
    {
        readonly RiderConnectable _riderConnectable;

        public TransportBusInstance(IBusControl busControl, IHost host, IHostConfiguration hostConfiguration)
        {
            BusControl = busControl;
            HostConfiguration = hostConfiguration;
            _riderConnectable = new RiderConnectable(host);
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }

        public IHostConfiguration HostConfiguration { get; }

        public void Connect<TRider>(IRiderControl riderControl)
            where TRider : IRider
        {
            _riderConnectable.Add<TRider>(riderControl);
        }

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            return _riderConnectable.Get<TRider>();
        }
    }
}
