namespace MassTransit.MessageData.Conventions
{
    using MassTransit.Configuration;
    using Topology;


    public class MessageDataSendTopologyConvention :
        ISendTopologyConvention
    {
        readonly ITopologyConventionCache<IMessageSendTopologyConvention> _cache;

        public MessageDataSendTopologyConvention(IMessageDataRepository repository)
        {
            _cache = new TopologyConventionCache<IMessageSendTopologyConvention>(typeof(MessageDataMessageSendTopologyConvention<>),
                new Factory(repository));
        }

        public bool TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
            where T : class
        {
            return _cache.GetOrAdd<T, IMessageSendTopologyConvention<T>>().TryGetMessageSendTopologyConvention(out convention);
        }


        class Factory :
            IConventionTypeFactory<IMessageSendTopologyConvention>
        {
            readonly IMessageDataRepository _repository;

            public Factory(IMessageDataRepository repository)
            {
                _repository = repository;
            }

            IMessageSendTopologyConvention IConventionTypeFactory<IMessageSendTopologyConvention>.Create<T>()
            {
                return new MessageDataMessageSendTopologyConvention<T>(_repository);
            }
        }
    }
}
