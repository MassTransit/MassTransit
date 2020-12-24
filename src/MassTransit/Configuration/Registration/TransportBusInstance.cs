namespace MassTransit.Registration
{
    using System;
    using Configuration;
    using Metadata;
    using Riders;


    public class TransportBusInstance :
        IBusInstance
    {
        readonly IHost _host;

        public TransportBusInstance(IBusControl busControl, IHost host, IHostConfiguration hostConfiguration)
        {
            _host = host;
            BusControl = busControl;
            HostConfiguration = hostConfiguration;
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }

        public IHostConfiguration HostConfiguration { get; }

        public void Connect<TRider>(IRiderControl riderControl)
            where TRider : IRider
        {
            var name = GetRiderName<TRider>();
            _host.AddRider(name, riderControl);
        }

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            var name = GetRiderName<TRider>();
            return (TRider)_host.GetRider(name);
        }

        static string GetRiderName<TRider>()
        {
            return TypeMetadataCache<TRider>.ShortName;
        }
    }
}
