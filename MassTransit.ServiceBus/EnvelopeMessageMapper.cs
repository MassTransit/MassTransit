namespace MassTransit.ServiceBus
{
    using System.Messaging;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Util;

    public class EnvelopeMessageMapper
    {
        private readonly static IFormatter _formatter = new BinaryFormatter();

        public static IEnvelope MapFrom(Message msg)
        {
            IEnvelope e;

            if (msg.ResponseQueue != null)
                e = new Envelope(MessageQueueEndpoint.Open(msg.ResponseQueue.Path));
            else
                e = new Envelope(); //TODO: How to get the endpoint in here

            e.Id = msg.Id;
            e.CorrelationId = msg.CorrelationId;

            e.TimeToBeReceived = msg.TimeToBeReceived;
            e.Recoverable = msg.Recoverable;
            e.Label = msg.Label;

            if (e.Id != MessageId.Empty)
            {
                e.SentTime = msg.SentTime;
                e.ArrivedTime = msg.ArrivedTime;
            }

            IMessage[] messages = _formatter.Deserialize(msg.BodyStream) as IMessage[];
            if (messages != null)
            {
                e.Messages = messages;
            }

            return e;
        }
        public static Message MapFrom(IEnvelope envelope)
        {
            Message msg = new Message();

            if (envelope.Messages != null && envelope.Messages.Length > 0)
            {
                _formatter.Serialize(msg.BodyStream, envelope.Messages);
            }

            msg.ResponseQueue = new MessageQueue(envelope.ReturnTo.Address);

            if (envelope.TimeToBeReceived < MessageQueue.InfiniteTimeout)
                msg.TimeToBeReceived = envelope.TimeToBeReceived;

            if (!string.IsNullOrEmpty(envelope.Label))
                msg.Label = envelope.Label;

            msg.Recoverable = envelope.Recoverable;

            if (envelope.CorrelationId != MessageId.Empty)
                msg.CorrelationId = envelope.CorrelationId;

            return msg;
        }
    }
}