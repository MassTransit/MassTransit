namespace MassTransit.Riders
{
    using System.Collections.Generic;
    using Metadata;


    public class RiderConnectable
    {
        readonly IHost _host;
        readonly object _mutateLock = new object();
        readonly Dictionary<string, IRider> _riders;

        public RiderConnectable(IHost host)
        {
            _host = host;
            _riders = new Dictionary<string, IRider>();
        }

        public void Add<TRider>(IRiderControl riderControl)
            where TRider : IRider
        {
            var name = GetRiderName<TRider>();
            lock (_mutateLock)
            {
                if (_riders.ContainsKey(name))
                    throw new ConfigurationException($"A rider with the same key was already added: {name}");

                _riders.Add(name, riderControl);
                _host.AddRider(name, riderControl);
            }
        }

        public TRider Get<TRider>()
            where TRider : IRider
        {
            var name = GetRiderName<TRider>();

            lock (_mutateLock)
            {
                if (!_riders.TryGetValue(name, out var r))
                    throw new ConfigurationException($"A rider with the key was not found: {name}");

                if (r is TRider rider)
                    return rider;

                throw new ConfigurationException($"A rider with the key is not compatible: {name}");
            }
        }

        static string GetRiderName<TRider>()
            where TRider : IRider
        {
            return TypeMetadataCache.GetShortName(typeof(TRider));
        }
    }
}
