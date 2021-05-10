namespace MassTransit.Logging
{
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using Newtonsoft.Json.Linq;


    public static class TransportDiagnosticSourceExtensions
    {
        public static void AddSendContextHeadersPostSend<T>(this StartedActivityContext activity, SendContext<T> context)
            where T : class
        {
            if (context is MessageSendContext<T> messageSendContext && messageSendContext.Serializer != null)
                activity?.AddTag(DiagnosticHeaders.BodyBytes, messageSendContext.BodyLength.ToString());
        }

        public static void AddConsumeContextHeaders(this StartedActivityContext activity, ConsumeContext context)
        {
            activity.AddTag(DiagnosticHeaders.MessageId, context.MessageId);
            activity.AddTag(DiagnosticHeaders.ConversationId, context.ConversationId);
            activity.AddTag(DiagnosticHeaders.CorrelationId, context.CorrelationId);
            activity.AddTag(DiagnosticHeaders.InitiatorId, context.InitiatorId);
            activity.AddTag(DiagnosticHeaders.SourceAddress, context.SourceAddress);
            activity.AddTag(DiagnosticHeaders.DestinationAddress, context.DestinationAddress);

            if (context.Host != null)
                activity.AddTag(DiagnosticHeaders.SourceHostMachine, context.Host.MachineName);

            if (context.SupportedMessageTypes != null)
                activity.AddTag(DiagnosticHeaders.MessageTypes, string.Join(",", context.SupportedMessageTypes));

            activity.AddBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId);
            activity.AddBaggage(DiagnosticHeaders.ConversationId, context.ConversationId);

            if (context.Headers.TryGetHeader(DiagnosticHeaders.ActivityCorrelationContext, out var correlationHeader))
            {
                foreach (KeyValuePair<string, string> value in GetValues(correlationHeader))
                {
                    if (!string.IsNullOrWhiteSpace(value.Value))
                        activity.AddBaggage(value.Key, value.Value);
                }
            }
        }

        static IEnumerable<KeyValuePair<string, string>> GetValues(object values)
        {
            return values switch
            {
                JArray array => array.ToObject<KeyValuePair<string, string>[]>(),
                IEnumerable<KeyValuePair<string, string>> enumerable => enumerable,
                _ => Enumerable.Empty<KeyValuePair<string, string>>()
            };
        }
    }
}
