namespace MassTransit.Logging
{
    using System.Collections.Generic;


    public static class TransportDiagnosticSourceExtensions
    {
        public static void AddSendContextHeadersPostSend<T>(this StartedActivityContext activity, SendContext<T> context)
            where T : class
        {
            if (context.BodyLength.HasValue)
                activity?.AddTag(DiagnosticHeaders.BodyBytes, context.BodyLength.Value.ToString());
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
                Dictionary<string, object> values = context.SerializerContext.ToDictionary(correlationHeader);
                foreach (KeyValuePair<string, object> value in values)
                {
                    if (value.Value is string text && !string.IsNullOrWhiteSpace(text))
                        activity.AddBaggage(value.Key, text);
                }
            }
        }
    }
}
