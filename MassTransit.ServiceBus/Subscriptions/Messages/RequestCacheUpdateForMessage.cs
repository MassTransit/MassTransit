namespace MassTransit.ServiceBus.Subscriptions.Messages
{
    using System;

    [Serializable]
    public class RequestCacheUpdateForMessage : IMessage
    {
        private string _messageName;


        public RequestCacheUpdateForMessage(Type message)
        {
            _messageName = message.FullName;
        }

        public RequestCacheUpdateForMessage(string messageName)
        {
            _messageName = messageName;
        }

        public string MessageName
        {
            get { return _messageName; }
        }
    }
}