namespace MassTransit.ServiceBus.Serialization
{
    using Newtonsoft.Json;

    public class JsonMessageEnvelope :
        BaseMessageEnvelope
    {
        
        public JsonMessageEnvelope()
        {
        }

        public JsonMessageEnvelope(object message)
        {
            Message = JavaScriptConvert.SerializeObject(message);
            MessageType = message.GetType().AssemblyQualifiedName;
        }

        public string Message { get; set; }
        
    }
}