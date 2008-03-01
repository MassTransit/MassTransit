namespace MassTransit.ServiceBus.Formatters
{
    using Newtonsoft.Json;

    public class JsonMessageFormatter
        : IMessageFormatter
    {
        public IMessageBody Serialize(IEnvelope env, IMessageBody body)
        {
            body.Body = JavaScriptConvert.SerializeObject(env.Messages);
            return body;
        }

        public IEnvelope Deserialize(IMessageBody messageBody)
        {
            return new Envelope(JavaScriptConvert.DeserializeObject<IMessage>((string)messageBody.Body));
        }
    }
}