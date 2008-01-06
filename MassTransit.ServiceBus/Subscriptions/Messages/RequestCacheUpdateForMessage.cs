namespace MassTransit.ServiceBus.Subscriptions.Messages
{
    using System;

    [Serializable]
    public class RequestCacheUpdateForMessage : IMessage
    {
        private Type _message;


        public RequestCacheUpdateForMessage(Type message)
        {
            _message = message;
        }


        public Type Message
        {
            get { return _message; }
        }
    }
}