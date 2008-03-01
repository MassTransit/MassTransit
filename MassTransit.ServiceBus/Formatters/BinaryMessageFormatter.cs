namespace MassTransit.ServiceBus.Formatters
{
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    public class BinaryMessageFormatter
        : IMessageFormatter
    {
        private static readonly IFormatter _formatter = new BinaryFormatter();

        public IMessageBody Serialize(IEnvelope env, IMessageBody body)
        {
            _formatter.Serialize(body.BodyStream, env.Messages);

            return body;
        }

        public IEnvelope Deserialize(IMessageBody messageBody)
        {
            IMessage[] messages = _formatter.Deserialize(messageBody.BodyStream) as IMessage[];

            return new Envelope(messages);
        }
    }
}