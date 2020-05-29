namespace MassTransit.Azure.ServiceBus.Core.Topology.Conventions.SessionId
{
    using MassTransit.Topology;
    using MassTransit.Topology.Conventions;


    public class SessionIdSendTopologyConvention :
        ISessionIdSendTopologyConvention
    {
        readonly IConventionTypeCache<IMessageSendTopologyConvention> _typeCache;

        public SessionIdSendTopologyConvention()
        {
            DefaultFormatter = new EmptySessionIdFormatter();

            _typeCache = new ConventionTypeCache<IMessageSendTopologyConvention>(typeof(ISessionIdMessageSendTopologyConvention<>), new CacheFactory());
        }

        bool IMessageSendTopologyConvention.TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
        {
            return _typeCache.GetOrAdd<T, IMessageSendTopologyConvention<T>>().TryGetMessageSendTopologyConvention(out convention);
        }

        public ISessionIdFormatter DefaultFormatter { get; set; }


        class CacheFactory :
            IConventionTypeCacheFactory<IMessageSendTopologyConvention>
        {
            IMessageSendTopologyConvention IConventionTypeCacheFactory<IMessageSendTopologyConvention>.Create<T>()
            {
                return new SessionIdMessageSendTopologyConvention<T>(null);
            }
        }
    }
}
