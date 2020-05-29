namespace MassTransit.Topology.Conventions
{
    using CorrelationId;


    /// <summary>
    /// Looks for a property that can be used as a CorrelationId message header, and
    /// applies a filter to set it on message send if available
    /// </summary>
    public class CorrelationIdSendTopologyConvention :
        ISendTopologyConvention
    {
        readonly IConventionTypeCache<IMessageSendTopologyConvention> _typeCache;

        public CorrelationIdSendTopologyConvention()
        {
            _typeCache = new ConventionTypeCache<IMessageSendTopologyConvention>(typeof(CorrelationIdMessageSendTopologyConvention<>), new CacheFactory());
        }

        public bool TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
            where T : class
        {
            return _typeCache.GetOrAdd<T, IMessageSendTopologyConvention<T>>().TryGetMessageSendTopologyConvention(out convention);
        }


        class CacheFactory :
            IConventionTypeCacheFactory<IMessageSendTopologyConvention>
        {
            IMessageSendTopologyConvention IConventionTypeCacheFactory<IMessageSendTopologyConvention>.Create<T>()
            {
                return new CorrelationIdMessageSendTopologyConvention<T>();
            }
        }
    }
}
