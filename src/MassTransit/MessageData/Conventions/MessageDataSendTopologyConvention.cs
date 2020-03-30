namespace MassTransit.MessageData.Conventions
{
    using Topology;
    using Topology.Conventions;


    public class MessageDataSendTopologyConvention :
        ISendTopologyConvention
    {
        readonly IConventionTypeCache<IMessageSendTopologyConvention> _typeCache;

        public MessageDataSendTopologyConvention(IMessageDataRepository repository)
        {
            _typeCache = new ConventionTypeCache<IMessageSendTopologyConvention>(typeof(MessageDataMessageSendTopologyConvention<>),
                new CacheFactory(repository));
        }

        public bool TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
            where T : class
        {
            return _typeCache.GetOrAdd<T, IMessageSendTopologyConvention<T>>().TryGetMessageSendTopologyConvention(out convention);
        }


        class CacheFactory :
            IConventionTypeCacheFactory<IMessageSendTopologyConvention>
        {
            readonly IMessageDataRepository _repository;

            public CacheFactory(IMessageDataRepository repository)
            {
                _repository = repository;
            }

            IMessageSendTopologyConvention IConventionTypeCacheFactory<IMessageSendTopologyConvention>.Create<T>()
            {
                return new MessageDataMessageSendTopologyConvention<T>(_repository);
            }
        }
    }
}
