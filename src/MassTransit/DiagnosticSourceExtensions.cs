namespace MassTransit
{
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using Logging;


    public static class DiagnosticSourceExtensions
    {
        public static void AddSendContextHeaders(this StartedActivity? startedActivity, SendContext context)
        {
            if (!startedActivity.HasValue)
                return;

            var activity = startedActivity.Value;

            context.Headers.Set(DiagnosticHeaders.ActivityId, activity.Activity.Id);

            if (activity.Activity.Baggage.Any())
                context.Headers.Set(DiagnosticHeaders.ActivityCorrelationContext, activity.Activity.Baggage.ToList());

            activity.AddTag(DiagnosticHeaders.MessageId, context.MessageId);
            activity.AddTag(DiagnosticHeaders.InitiatorId, context.InitiatorId);
            activity.AddTag(DiagnosticHeaders.SourceAddress, context.SourceAddress);
            activity.AddTag(DiagnosticHeaders.DestinationAddress, context.DestinationAddress);

            activity.AddBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId);
            activity.AddBaggage(DiagnosticHeaders.ConversationId, context.ConversationId);
        }

        public static void AddReceiveContextHeaders(this StartedActivity? startedActivity, ReceiveContext context)
        {
            if (!startedActivity.HasValue)
                return;

            var activity = startedActivity.Value;

            activity.AddTag(DiagnosticHeaders.InputAddress, context.InputAddress);

            if (context.TransportHeaders.TryGetHeader("MessageId", out var messageIdHeader) && messageIdHeader != null)
                activity.AddTag(DiagnosticHeaders.MessageId, messageIdHeader.ToString());

            context.AddOrUpdatePayload<StartedActivityContext>(() => activity, _ => activity);
        }

        public static void AddConsumeContextHeaders(this StartedActivityContext activity, ConsumeContext context)
        {
            activity.AddTag(DiagnosticHeaders.MessageId, context.MessageId);
            activity.AddTag(DiagnosticHeaders.InitiatorId, context.InitiatorId);
            activity.AddTag(DiagnosticHeaders.SourceAddress, context.SourceAddress);
            activity.AddTag(DiagnosticHeaders.DestinationAddress, context.DestinationAddress);

            if (context.Host != null)
            {
                activity.AddTag(DiagnosticHeaders.SourceHostMachine, context.Host.MachineName);
                activity.AddTag(DiagnosticHeaders.SourceHostFrameworkVersion, context.Host.FrameworkVersion);
                activity.AddTag(DiagnosticHeaders.SourceHostProcessId, context.Host.ProcessId.ToString());
                activity.AddTag(DiagnosticHeaders.SourceHostMassTransitVersion, context.Host.MassTransitVersion);
            }

            if (context.SupportedMessageTypes != null)
                activity.AddTag(DiagnosticHeaders.MessageTypes, string.Join(",", context.SupportedMessageTypes));

            activity.AddBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId);
            activity.AddBaggage(DiagnosticHeaders.ConversationId, context.ConversationId);

            if (context.Headers.TryGetHeader(DiagnosticHeaders.ActivityId, out var activityIdHeader)
                && activityIdHeader is string activityId && !string.IsNullOrWhiteSpace(activityId))
            {
                activity.SetParentId(activityId);
            }

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
