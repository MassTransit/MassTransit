namespace MassTransit.Registration
{
    using System;
    using Configuration;
    using GreenPipes;
    using Riders;


    public class TransportBusInstance :
        IBusInstance
    {
        readonly RiderConnectable _riders;

        public TransportBusInstance(IBusControl busControl, IHost host, IHostConfiguration hostConfiguration)
            : this(busControl, host, hostConfiguration, new RiderConnectable())
        {
        }

        public TransportBusInstance(IBusControl busControl, IHost host, IHostConfiguration hostConfiguration, RiderConnectable riders)
        {
            _riders = riders;
            BusControl = busControl;
            Host = host;
            HostConfiguration = hostConfiguration;
        }

        public IHost Host { get; }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }

        public IHostConfiguration HostConfiguration { get; }

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            var rider = _riders.Get<TRider>();
            if (rider == null)
                throw new ConfigurationException($"Rider: {typeof(TRider).Name} is not registered.");
            return rider;
        }

        public ConnectHandle ConnectRider(IRider rider)
        {
            return _riders.Connect(rider);
        }
    }
}
