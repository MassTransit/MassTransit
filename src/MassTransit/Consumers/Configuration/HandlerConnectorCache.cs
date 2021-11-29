namespace MassTransit.Configuration
{
    using System;


    public class HandlerConnectorCache<TMessage> :
        IHandlerConnectorCache<TMessage>
        where TMessage : class
    {
        readonly HandlerConnector<TMessage> _connector;

        HandlerConnectorCache()
        {
            _connector = new HandlerConnector<TMessage>();
        }

        public static IHandlerConnector<TMessage> Connector => InstanceCache.Cached.Value.Connector;

        IHandlerConnector<TMessage> IHandlerConnectorCache<TMessage>.Connector => _connector;


        static class InstanceCache
        {
            internal static readonly Lazy<IHandlerConnectorCache<TMessage>> Cached = new Lazy<IHandlerConnectorCache<TMessage>>(
                () => new HandlerConnectorCache<TMessage>());
        }
    }
}
