namespace MassTransit.ServiceBus.SubscriptionsManager.Messages
{
    using System;

    [Serializable]
    public class RequestSubscribersForMessage : IMessage
    {
        private Type _message;


        public RequestSubscribersForMessage(Type message)
        {
            _message = message;
        }


        public Type Message
        {
            get { return _message; }
        }
    }
}