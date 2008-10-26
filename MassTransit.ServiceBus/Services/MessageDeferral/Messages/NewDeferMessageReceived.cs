namespace MassTransit.ServiceBus.Services.MessageDeferral.Messages
{
    using System;

    [Serializable]
    public class NewDeferMessageReceived
    {
        private readonly Guid _id;
        private readonly DateTime _deliverAt;
        private readonly string _messageType;


        public NewDeferMessageReceived(Guid id, DateTime deliverAt, string messageType)
        {
            _id = id;
            _deliverAt = deliverAt;
            _messageType = messageType;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public DateTime DeliverAt
        {
            get { return _deliverAt; }
        }

        public string MessageType
        {
            get { return _messageType; }
        }
    }
}