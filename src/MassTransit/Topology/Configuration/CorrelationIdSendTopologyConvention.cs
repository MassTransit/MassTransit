namespace MassTransit.Configuration
{
    /// <summary>
    /// Looks for a property that can be used as a CorrelationId message header, and
    /// applies a filter to set it on message send if available
    /// </summary>
    public class CorrelationIdSendTopologyConvention :
        ISendTopologyConvention
    {
        readonly ITopologyConventionCache<IMessageSendTopologyConvention> _cache;

        public CorrelationIdSendTopologyConvention()
        {
            _cache = new TopologyConventionCache<IMessageSendTopologyConvention>(typeof(CorrelationIdMessageSendTopologyConvention<>), new Factory());
        }

        public bool TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
            where T : class
        {
            return _cache.GetOrAdd<T, IMessageSendTopologyConvention<T>>().TryGetMessageSendTopologyConvention(out convention);
        }


        class Factory :
            IConventionTypeFactory<IMessageSendTopologyConvention>
        {
            IMessageSendTopologyConvention IConventionTypeFactory<IMessageSendTopologyConvention>.Create<T>()
            {
                return new CorrelationIdMessageSendTopologyConvention<T>();
            }
        }
    }
}
