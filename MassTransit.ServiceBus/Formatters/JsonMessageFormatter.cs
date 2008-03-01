namespace MassTransit.ServiceBus.Formatters
{
    using Newtonsoft.Json;

    public class JsonMessageFormatter
        : IMessageFormatter
    {
        public object Serialize(IEnvelope env)
        {
            return JavaScriptConvert.SerializeObject(env);
        }

        public IEnvelope Deserialize(object messageBody)
        {
            return JavaScriptConvert.DeserializeObject<IEnvelope>((string)messageBody);
        }
    }
}