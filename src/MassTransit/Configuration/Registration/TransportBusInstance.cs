namespace MassTransit.Registration
{
    using System;
    using Configuration;
    using GreenPipes;
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

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            return _riderConnectable.Get<TRider>();
        }

        public ConnectHandle Connect(IRider rider)
        {
            return _riderConnectable.Connect(rider);
        }
    }
}
