namespace MassTransit.TestFramework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using GreenPipes;
    using GreenPipes.Introspection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public static class IntrospectionExtensions
    {
        public static IEnumerable<Uri> GetReceiveEndpointAddresses(this IBus bus)
        {
            var probeResult = bus.GetProbeResult();

            var probeJObject = JObject.Parse(probeResult.ToJsonString());
            JEnumerable<JToken> receiveEndpoints = probeJObject["results"]["bus"]["host"]["receiveEndpoint"].Children();

            IEnumerable<ReceiveTransportProbeResult> probeResults = receiveEndpoints.Select(result =>
                    JsonConvert.DeserializeObject<ReceiveTransportProbeResult>(result["receiveTransport"].ToString()))
                .Where(x => x.Address != null);

            return probeResults.Select(result => new Uri(result.Address));
        }

        public static string ToJsonString(this ProbeResult result)
        {
            var encoding = new UTF8Encoding(false, true);

            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, encoding, 1024, true);
            using var jsonWriter = new JsonTextWriter(writer) {Formatting = Formatting.Indented};

            SerializerCache.Serializer.Serialize(jsonWriter, result, typeof(ProbeResult));

            jsonWriter.Flush();
            writer.Flush();

            return encoding.GetString(stream.ToArray());
        }


        class ReceiveTransportProbeResult
        {
            public string Address { get; set; }
        }
    }
}
