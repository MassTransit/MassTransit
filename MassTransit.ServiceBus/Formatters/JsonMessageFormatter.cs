namespace MassTransit.ServiceBus.Formatters
{
    using Newtonsoft.Json;

    public class JsonMessageFormatter
        : IMessageFormatter
    {
        public object Serialize(IEnvelope env)
        {
            return JavaScriptConvert.SerializeObject(env.Messages);
        }

        public IEnvelope Deserialize(object messageBody)
        {
            return new Envelope(JavaScriptConvert.DeserializeObject<IMessage>((string)messageBody));
        }
    }
}