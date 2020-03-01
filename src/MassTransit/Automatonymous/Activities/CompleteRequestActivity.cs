namespace Automatonymous.Activities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Contracts;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Requests;


    public class CompleteRequestActivity :
        Activity<RequestState, RequestCompleted>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("completeRequest");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<RequestState, RequestCompleted> context, Behavior<RequestState, RequestCompleted> next)
        {
            if (!context.Instance.ExpirationTime.HasValue || context.Instance.ExpirationTime.Value > DateTime.UtcNow)
            {
                if (!context.TryGetPayload(out ConsumeContext<RequestCompleted> consumeContext))
                    throw new ArgumentException("The ConsumeContext was not present", nameof(context));

                string body = GetResponseBody(consumeContext, context.Instance);

                IPipe<SendContext> pipe = new CompletedMessagePipe(context.GetPayload<ConsumeContext<RequestCompleted>>(), context.Instance, body);

                var endpoint = await context.GetSendEndpoint(context.Instance.ResponseAddress).ConfigureAwait(false);

                var scheduled = new CompletedEvent();

                await endpoint.Send(scheduled, pipe).ConfigureAwait(false);
            }

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<RequestState, RequestCompleted, TException> context,
            Behavior<RequestState, RequestCompleted> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        static string GetResponseBody(ConsumeContext<RequestCompleted> context, RequestState requestState)
        {
            string body = Encoding.UTF8.GetString(context.ReceiveContext.GetBody());

            var mediaType = context.ReceiveContext.ContentType?.MediaType;

            if (JsonMessageSerializer.JsonContentType.MediaType.Equals(mediaType, StringComparison.OrdinalIgnoreCase))
                return TranslateJsonBody(body, requestState);

            if (XmlMessageSerializer.XmlContentType.MediaType.Equals(mediaType, StringComparison.OrdinalIgnoreCase))
                return TranslateXmlBody(body, requestState);

            throw new InvalidOperationException("Only JSON and XML messages can be scheduled");
        }

        static string TranslateJsonBody(string body, RequestState requestState)
        {
            JObject envelope = JObject.Parse(body);

            envelope["destinationAddress"] = requestState.ResponseAddress.ToString();

            JToken message = envelope["message"];

            JToken payload = message["payload"];
            JToken payloadType = message["payloadType"];

            envelope["message"] = payload;
            envelope["messageType"] = payloadType;

            if (requestState.ConversationId.HasValue)
                envelope["conversationId"] = requestState.ConversationId.Value.ToString("D");

            envelope["sourceAddress"] = requestState.SagaAddress.ToString();
            envelope["requestId"] = requestState.CorrelationId.ToString("D");

            if (requestState.FaultAddress != null)
                envelope["faultAddress"] = requestState.FaultAddress.ToString();

            return JsonConvert.SerializeObject(envelope, Formatting.Indented);
        }

        static string TranslateXmlBody(string body, RequestState requestState)
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

                if (requestState.ConversationId.HasValue)
                {
                    XElement conversationId = (from a in envelope.Descendants("conversationId") select a).Single();
                    conversationId.Value = requestState.ConversationId.Value.ToString("D");
                }

                destinationAddress.Value = requestState.ResponseAddress.ToString();

                XElement sourceAddress = (from a in envelope.Descendants("sourceAddress") select a).Single();
                sourceAddress.Value = requestState.SagaAddress.ToString();

                message = new XElement("message");
                message.Add(payload.Descendants());
                envelope.Add(message);

                envelope.Add(payloadType.Select(x => new XElement("messageType", x.Value)));

                envelope.Add(new XElement("requestId", requestState.CorrelationId.ToString("D")));
                if (requestState.FaultAddress != null)
                    envelope.Add(new XElement("faultAddress", requestState.FaultAddress.ToString()));

                return document.ToString(SaveOptions.DisableFormatting);
            }
        }


        class CompletedEvent
        {
        }


        class CompletedMessagePipe :
            IPipe<SendContext>
        {
            readonly ConsumeContext<RequestCompleted> _context;
            readonly RequestState _instance;
            readonly string _body;

            public CompletedMessagePipe(ConsumeContext<RequestCompleted> context, RequestState instance, string body)
            {
                _context = context;
                _instance = instance;
                _body = body;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
            }

            public async Task Send(SendContext context)
            {
                context.DestinationAddress = _instance.ResponseAddress;
                context.SourceAddress = _instance.SagaAddress;
                context.FaultAddress = _instance.FaultAddress;
                context.RequestId = _instance.CorrelationId;

                if (_instance.ExpirationTime.HasValue)
                {
                    var timeToLive = DateTime.UtcNow - _instance.ExpirationTime.Value;
                    context.TimeToLive = timeToLive > TimeSpan.Zero ? timeToLive : TimeSpan.FromSeconds(1);
                }

                var bodySerializer = new StringMessageSerializer(_context.ReceiveContext.ContentType, _body);

                context.Serializer = bodySerializer;
            }
        }
    }
}
