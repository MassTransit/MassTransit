namespace MassTransit.MessageData.Conventions
{
    using MassTransit.Configuration;


    public class MessageDataConsumeTopologyConvention :
        IConsumeTopologyConvention
    {
        readonly ITopologyConventionCache<IMessageConsumeTopologyConvention> _cache;

        public MessageDataConsumeTopologyConvention(IMessageDataRepository repository)
        {
            _cache = new TopologyConventionCache<IMessageConsumeTopologyConvention>(typeof(MessageDataMessageConsumeTopologyConvention<>),
                new Factory(repository));
        }

        public bool TryGetMessageConsumeTopologyConvention<T>(out IMessageConsumeTopologyConvention<T> convention)
            where T : class
        {
            return _cache.GetOrAdd<T, IMessageConsumeTopologyConvention<T>>().TryGetMessageConsumeTopologyConvention(out convention);
        }


        class Factory :
            IConventionTypeFactory<IMessageConsumeTopologyConvention>
        {
            readonly IMessageDataRepository _repository;

            public Factory(IMessageDataRepository repository)
            {
                _repository = repository;
            }

            IMessageConsumeTopologyConvention IConventionTypeFactory<IMessageConsumeTopologyConvention>.Create<T>()
            {
                return new MessageDataMessageConsumeTopologyConvention<T>(_repository);
            }
        }
    }
}
