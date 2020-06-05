namespace MassTransit.Registration
{
    using System;
    using Configuration;
    using GreenPipes;
    using Riders;


    public class DefaultBusInstance :
        IBusInstance
    {
        public DefaultBusInstance(IBusControl busControl)
        {
            BusControl = busControl;
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }

        public IHostConfiguration HostConfiguration => default;

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            throw new ConfigurationException("TODO: add a link to a documentation");
        }

        public ConnectHandle ConnectRider(IRider rider)
        {
            throw new ConfigurationException("TODO: add a link to a documentation");
        }
    }
}
