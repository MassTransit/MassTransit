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
            //which
            _formatter.Serialize(null, env.Messages);
            _formatter.Serialize(null, env);

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