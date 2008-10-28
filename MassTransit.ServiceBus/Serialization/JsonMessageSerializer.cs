namespace MassTransit.ServiceBus.Serialization
{
    using System;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    public class JsonMessageSerializer :
        IMessageSerializer
    {
        public void Serialize<T>(Stream output, T message)
        {
            var envelope = new JsonMessageEnvelope(message);
            string wrappedJson = JavaScriptConvert.SerializeObject(envelope);

            var mstream = new MemoryStream(Encoding.UTF8.GetBytes(wrappedJson));

            mstream.WriteTo(output);
        }

        public object Deserialize(Stream input)
        {
            var buffer = new byte[input.Length];

            input.Read(buffer, 0, buffer.Length);
            string body = Encoding.UTF8.GetString(buffer);

            var envelope = JavaScriptConvert.DeserializeObject<JsonMessageEnvelope>(body);

            Type t = Type.GetType(envelope.MessageType, true, true);
            var message = JavaScriptConvert.DeserializeObject(envelope.Message, t);

            return message;
        }
    }
}