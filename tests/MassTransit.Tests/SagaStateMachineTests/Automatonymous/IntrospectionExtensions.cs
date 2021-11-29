namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System.IO;
    using System.Text;
    using Introspection;
    using Newtonsoft.Json;


    public static class IntrospectionExtensions
    {
        public static string ToJsonString(this ProbeResult result)
        {
            var encoding = new UTF8Encoding(false, true);

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, encoding, 1024, true))
                {
                    using (var jsonWriter = new JsonTextWriter(writer))
                    {
                        jsonWriter.Formatting = Formatting.Indented;

                        SerializerCache.Serializer.Serialize(jsonWriter, result, typeof(ProbeResult));

                        jsonWriter.Flush();
                        writer.Flush();

                        return encoding.GetString(stream.ToArray());
                    }
                }
            }
        }
    }
}
