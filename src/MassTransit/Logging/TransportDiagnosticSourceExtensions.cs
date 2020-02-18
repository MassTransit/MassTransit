namespace MassTransit.Logging
{
    using System.Collections.Generic;
    using Context;


    public static class TransportDiagnosticSourceExtensions
    {
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

            if (context.Headers.TryGetHeader(DiagnosticHeaders.ActivityCorrelationContext, out var correlationHeader)
                && correlationHeader is IEnumerable<KeyValuePair<string, string>> correlationValues)
            {
                foreach (KeyValuePair<string, string> value in correlationValues)
                {
                    if (!string.IsNullOrWhiteSpace(value.Value))
                        activity.AddBaggage(value.Key, value.Value);
                }
            }
        }
    }
}
