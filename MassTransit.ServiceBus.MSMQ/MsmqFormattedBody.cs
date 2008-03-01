namespace MassTransit.ServiceBus.MSMQ
{
    using System.IO;
    using System.Messaging;
    using Formatters;

    public class MsmqFormattedBody
        : IFormattedBody
    {
        private Message _wrappedMessage;


        public MsmqFormattedBody(Message wrappedMessage)
        {
            _wrappedMessage = wrappedMessage;
        }

        public object Body
        {
            get { return _wrappedMessage.Body; }
            set { _wrappedMessage.Body = value; }
        }

        public Stream BodyStream
        {
            get { return _wrappedMessage.BodyStream; }
            set { _wrappedMessage.BodyStream = value; }
        }
    }
}