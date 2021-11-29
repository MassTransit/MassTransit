namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using MassTransit.Configuration;


    public class SessionIdSendTopologyConvention :
        ISessionIdSendTopologyConvention
    {
        readonly ITopologyConventionCache<IMessageSendTopologyConvention> _cache;

        public SessionIdSendTopologyConvention()
        {
            DefaultFormatter = new EmptySessionIdFormatter();

            _cache = new TopologyConventionCache<IMessageSendTopologyConvention>(typeof(ISessionIdMessageSendTopologyConvention<>), new Factory());
        }

        bool IMessageSendTopologyConvention.TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
        {
            return _cache.GetOrAdd<T, IMessageSendTopologyConvention<T>>().TryGetMessageSendTopologyConvention(out convention);
        }

        public ISessionIdFormatter DefaultFormatter { get; set; }


        class Factory :
            IConventionTypeFactory<IMessageSendTopologyConvention>
        {
            IMessageSendTopologyConvention IConventionTypeFactory<IMessageSendTopologyConvention>.Create<T>()
            {
                return new SessionIdMessageSendTopologyConvention<T>(null);
            }
        }
    }
}
