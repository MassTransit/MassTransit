namespace MassTransit.Steward.Core.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Contracts;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Serialization;


    public class DispatchMessageConsumer :
        IConsumer<DispatchMessage>
    {
        readonly DispatchAgent _agent;

        public DispatchMessageConsumer(DispatchAgent agent)
        {
            _agent = agent;
        }

        public async Task Consume(ConsumeContext<DispatchMessage> context)
        {
            string body = Encoding.UTF8.GetString(context.ReceiveContext.GetBody());

            var mediaType = context.ReceiveContext.ContentType?.MediaType;
            if (JsonMessageSerializer.JsonContentType.MediaType.Equals(mediaType, StringComparison.OrdinalIgnoreCase))
                body = TranslateJsonBody(body, context.Message.Destination.ToString());

            else if (XmlMessageSerializer.XmlContentType.MediaType.Equals(mediaType, StringComparison.OrdinalIgnoreCase))
                body = TranslateXmlBody(body, context.Message.Destination.ToString());
            else
                throw new InvalidOperationException("Only JSON and XML messages can be scheduled");

            var dispatchContext = new ConsumerDispatchContext(context, body);

            await _agent.Execute(dispatchContext);
        }

        static string TranslateJsonBody(string body, string destination)
        {
            JObject envelope = JObject.Parse(body);

            envelope["destinationAddress"] = destination;

            JToken message = envelope["message"];

            JToken payload = message["payload"];
            JToken payloadType = message["payloadType"];

            envelope["message"] = payload;
            envelope["messageType"] = payloadType;

            return JsonConvert.SerializeObject(envelope, Formatting.Indented);
        }

        static string TranslateXmlBody(string body, string destination)
        {
            using (var reader = new StringReader(body))
            {
                XDocument document = XDocument.Load(reader);

                XElement envelope = (from e in document.Descendants("envelope") select e).Single();

                XElement destinationAddress = (from a in envelope.Descendants("destinationAddress") select a).Single();

                XElement message = (from m in envelope.Descendants("message") select m).Single();
                IEnumerable<XElement> messageType = (from mt in envelope.Descendants("messageType") select mt);

                XElement payload = (from p in message.Descendants("payload") select p).Single();
                IEnumerable<XElement> payloadType = (from pt in message.Descendants("payloadType") select pt);

                message.Remove();
                messageType.Remove();

                destinationAddress.Value = destination;

                message = new XElement("message");
                message.Add(payload.Descendants());
                envelope.Add(message);

                envelope.Add(payloadType.Select(x => new XElement("messageType", x.Value)));

                return document.ToString(SaveOptions.DisableFormatting);
            }
        }
    }
}
