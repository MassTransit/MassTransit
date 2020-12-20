namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Riders;


    public class TransportBusInstance :
        IBusInstance
    {
        readonly List<IRider> _riders;

        public TransportBusInstance(IBusControl busControl, IHost host, IHostConfiguration hostConfiguration)
        {
            BusControl = busControl;
            Host = host;
            HostConfiguration = hostConfiguration;
            _riders = new List<IRider>();
        }

        public IHost Host { get; }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }

        public IHostConfiguration HostConfiguration { get; }

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            var rider = _riders.OfType<TRider>().FirstOrDefault();
            if (rider == null)
                throw new ConfigurationException($"Rider: {typeof(TRider).Name} is not registered.");
            return rider;
        }

        public void ConnectRider(IRider rider)
        {
            _riders.Add(rider);
            rider.Connect(Host);
        }
    }
}
