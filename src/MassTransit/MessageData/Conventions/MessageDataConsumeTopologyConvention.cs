namespace MassTransit.MessageData.Conventions
{
    using Topology;
    using Topology.Conventions;


    public class MessageDataConsumeTopologyConvention :
        IConsumeTopologyConvention
    {
        readonly IConventionTypeCache<IMessageConsumeTopologyConvention> _typeCache;

        public MessageDataConsumeTopologyConvention(IMessageDataRepository repository)
        {
            _typeCache = new ConventionTypeCache<IMessageConsumeTopologyConvention>(typeof(MessageDataMessageConsumeTopologyConvention<>),
                new CacheFactory(repository));
        }

        public bool TryGetMessageConsumeTopologyConvention<T>(out IMessageConsumeTopologyConvention<T> convention)
            where T : class
        {
            return _typeCache.GetOrAdd<T, IMessageConsumeTopologyConvention<T>>().TryGetMessageConsumeTopologyConvention(out convention);
        }


        class CacheFactory :
            IConventionTypeCacheFactory<IMessageConsumeTopologyConvention>
        {
            readonly IMessageDataRepository _repository;

            public CacheFactory(IMessageDataRepository repository)
            {
                _repository = repository;
            }

            IMessageConsumeTopologyConvention IConventionTypeCacheFactory<IMessageConsumeTopologyConvention>.Create<T>()
            {
                return new MessageDataMessageConsumeTopologyConvention<T>(_repository);
            }
        }
    }
}
