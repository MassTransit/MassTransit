namespace MassTransit.ServiceBus.Formatters
{
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    public class BinaryMessageFormatter
        : IMessageFormatter
    {
        private static readonly IFormatter _formatter = new BinaryFormatter();

        public object Serialize(IEnvelope env)
        {
            _formatter.Serialize(null, env.Messages);

            return null;
        }

        public IEnvelope Deserialize(object messageBody)
        {
            //IMessage[] messages = _formatter.Deserialize(msg.BodyStream) as IMessage[];
            IMessage[] messages = _formatter.Deserialize(null) as IMessage[];

            return new Envelope(messages);
        }
    }
}