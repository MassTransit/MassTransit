namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Scheduling;
    using Serialization;


    class HangfireScheduledMessageData :
        SerializedMessage
    {
        public string DestinationAddress { get; set; }
        public Uri Destination => new Uri(DestinationAddress);
        public string ContentType { get; set; }
        public string ExpirationTime { get; set; }
        public string ResponseAddress { get; set; }
        public string FaultAddress { get; set; }
        public string Body { get; set; }
        public string MessageId { get; set; }
        public string RequestId { get; set; }
        public string CorrelationId { get; set; }
        public string ConversationId { get; set; }
        public string InitiatorId { get; set; }
        public string HeadersAsJson { get; set; }
        public string PayloadMessageHeadersAsJson { get; set; }

        public static HangfireScheduledMessageData Create(ConsumeContext<ScheduleMessage> context)
        {
            var message = new HangfireScheduledMessageData();

            SetBaseProperties(message, context, context.Message.Destination);

            return message;
        }

        protected static void SetBaseProperties(HangfireScheduledMessageData message, ConsumeContext context, Uri destination)
        {
            message.DestinationAddress = destination?.ToString() ?? "";
            message.Body = ExtractBody(context.ReceiveContext.ContentType?.MediaType, context.ReceiveContext.GetBody(), destination);
            message.ContentType = context.ReceiveContext.ContentType?.MediaType;
            message.FaultAddress = context.FaultAddress?.ToString() ?? "";
            message.ResponseAddress = context.ResponseAddress?.ToString() ?? "";

            if (context.MessageId.HasValue)
                message.MessageId = context.MessageId.Value.ToString();

            if (context.CorrelationId.HasValue)
                message.CorrelationId = context.CorrelationId.Value.ToString();

            if (context.ConversationId.HasValue)
                message.ConversationId = context.ConversationId.Value.ToString();

            if (context.InitiatorId.HasValue)
                message.InitiatorId = context.InitiatorId.Value.ToString();

            if (context.RequestId.HasValue)
                message.RequestId = context.RequestId.Value.ToString();

            if (context.ExpirationTime.HasValue)
                message.ExpirationTime = context.ExpirationTime.Value.ToString("O");

            IEnumerable<KeyValuePair<string, object>> headers = context.Headers.GetAll();
            if (headers.Any())
                message.HeadersAsJson = JsonConvert.SerializeObject(headers);
        }

        protected static string ExtractBody(string mediaType, byte[] bytes, Uri destination)
        {
            var body = Encoding.UTF8.GetString(bytes);
            if (JsonMessageSerializer.JsonContentType.MediaType.Equals(mediaType, StringComparison.OrdinalIgnoreCase))
                body = TranslateJsonBody(body, destination.ToString());
            else if (XmlMessageSerializer.XmlContentType.MediaType.Equals(mediaType, StringComparison.OrdinalIgnoreCase))
                body = TranslateXmlBody(body, destination.ToString());
            else
                throw new InvalidOperationException("Only JSON and XML messages can be scheduled");
            return body;
        }

        static string TranslateJsonBody(string body, string destination)
        {
            var envelope = JObject.Parse(body);

            envelope["destinationAddress"] = destination;

            var message = envelope["message"];

            var payload = message["payload"];
            var payloadType = message["payloadType"];

            envelope["message"] = payload;
            envelope["messageType"] = payloadType;

            return JsonConvert.SerializeObject(envelope, Formatting.Indented);
        }

        static string TranslateXmlBody(string body, string destination)
        {
            using var reader = new StringReader(body);
            var document = XDocument.Load(reader);

            var envelope = (from e in document.Descendants("envelope") select e).Single();

            var destinationAddress = (from a in envelope.Descendants("destinationAddress") select a).Single();

            var message = (from m in envelope.Descendants("message") select m).Single();
            IEnumerable<XElement> messageType = (from mt in envelope.Descendants("messageType") select mt);

            var payload = (from p in message.Descendants("payload") select p).Single();
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
