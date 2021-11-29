namespace MassTransit.Configuration
{
    using System;
    using System.Threading;


    public class ObserverConnectorCache<TMessage> :
        IObserverConnectorCache<TMessage>
        where TMessage : class
    {
        readonly Lazy<MessageObserverConnector<TMessage>> _connector;

        ObserverConnectorCache()
        {
            _connector = new Lazy<MessageObserverConnector<TMessage>>(() => new MessageObserverConnector<TMessage>());
        }

        public static IObserverConnector<TMessage> Connector => InstanceCache.Cached.Value.Connector;

        IObserverConnector<TMessage> IObserverConnectorCache<TMessage>.Connector => _connector.Value;


        static class InstanceCache
        {
            internal static readonly Lazy<IObserverConnectorCache<TMessage>> Cached = new Lazy<IObserverConnectorCache<TMessage>>(
                () => new ObserverConnectorCache<TMessage>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
