namespace MassTransit.Transports
{
    using System;
    using Configuration;


    static class SendEndpointCacheFactory
    {
        static readonly bool _useInternalCache = AppContext.TryGetSwitch(AppContextSwitches.UseInternalCache, out var isEnabled) && isEnabled;
        static readonly bool _useLegacyCache = AppContext.TryGetSwitch(AppContextSwitches.UseLegacyCache, out var isEnabled) && isEnabled;

        internal static ISendEndpointCache<T> Create<T>()
        {
            return _useInternalCache || !_useLegacyCache
                ? (ISendEndpointCache<T>)new SendEndpointMassTransitCache<T>()
                : new SendEndpointCache<T>();
        }
    }
}
