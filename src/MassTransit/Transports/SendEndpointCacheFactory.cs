namespace MassTransit.Transports
{
    using System;
    using Configuration;


    static class SendEndpointCacheFactory
    {
        static readonly bool _useInternalCache = AppContext.TryGetSwitch(AppContextSwitches.UseInternalCache, out var isEnabled) && isEnabled;

        internal static ISendEndpointCache<T> Create<T>()
        {
            return _useInternalCache
                ? (ISendEndpointCache<T>)new SendEndpointMassTransitCache<T>()
                : new SendEndpointCache<T>();
        }
    }
}
