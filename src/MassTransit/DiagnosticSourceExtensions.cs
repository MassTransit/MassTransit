namespace MassTransit
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Logging;


    public static class DiagnosticSourceExtensions
    {
        public static EnabledDiagnosticSource? IfEnabled(this DiagnosticSource source, string name)
        {
            return source.IsEnabled(name) ? new EnabledDiagnosticSource(source, name) : default(EnabledDiagnosticSource?);
        }

        public static void IfEnabled(this DiagnosticSource source, Activity activity, object args = null)
        {
            if (activity != null)
                source.StopActivity(activity, args);
        }

        public static StartedActivity? AddSendContextHeaders(this StartedActivity? startedActivity, SendContext context)
        {
            if (!startedActivity.HasValue)
                return startedActivity;

            var activity = startedActivity.Value.Activity;

            context.Headers.Set(DiagnosticHeaders.ActivityId, activity.Id);

            if (activity.Baggage.Any())
                context.Headers.Set(DiagnosticHeaders.ActivityCorrelationContext, activity.Baggage.ToList());

            if (context.MessageId.HasValue)
                activity.AddTag(DiagnosticHeaders.MessageId, context.MessageId.Value.ToString());
            if (context.InitiatorId.HasValue)
                activity.AddTag(DiagnosticHeaders.InitiatorId, context.InitiatorId.Value.ToString());
            if (context.SourceAddress != null)
                activity.AddTag(DiagnosticHeaders.SourceAddress, context.SourceAddress.ToString());
            if (context.DestinationAddress != null)
                activity.AddTag(DiagnosticHeaders.DestinationAddress, context.DestinationAddress.ToString());

            if (context.CorrelationId.HasValue)
                activity.AddBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString());
            if (context.ConversationId.HasValue)
                activity.AddBaggage(DiagnosticHeaders.CorrelationConversationId, context.ConversationId.Value.ToString());

            return startedActivity;
        }

        public static StartedActivity? AddReceiveContextHeaders(this StartedActivity? startedActivity, ReceiveContext context)
        {
            if (!startedActivity.HasValue)
                return startedActivity;

            var activity = startedActivity.Value.Activity;

            activity.AddTag(DiagnosticHeaders.InputAddress, context.InputAddress.ToString());

            if (context.TransportHeaders.TryGetHeader("MessageId", out var messageIdHeader) && messageIdHeader != null)
                activity.AddTag(DiagnosticHeaders.MessageId, messageIdHeader.ToString());

            context.AddOrUpdatePayload(() => activity, _ => activity);

            return startedActivity;
        }

        public static Activity AddConsumeContextHeaders(this Activity activity, ConsumeContext context)
        {
            if (context.MessageId.HasValue)
                activity.AddTag(DiagnosticHeaders.MessageId, context.MessageId.Value.ToString());
            if (context.InitiatorId.HasValue)
                activity.AddTag(DiagnosticHeaders.InitiatorId, context.InitiatorId.Value.ToString());
            if (context.SourceAddress != null)
                activity.AddTag(DiagnosticHeaders.SourceAddress, context.SourceAddress.ToString());
            if (context.DestinationAddress != null)
                activity.AddTag(DiagnosticHeaders.DestinationAddress, context.DestinationAddress.ToString());
            if (context.Host != null)
            {
                activity.AddTag(DiagnosticHeaders.SourceHostMachine, context.Host.MachineName);
                activity.AddTag(DiagnosticHeaders.SourceHostFrameworkVersion, context.Host.FrameworkVersion);
                activity.AddTag(DiagnosticHeaders.SourceHostProcessId, context.Host.ProcessId.ToString());
                activity.AddTag(DiagnosticHeaders.SourceHostMassTransitVersion, context.Host.MassTransitVersion);
            }

            if (context.SupportedMessageTypes != null)
                activity.AddTag(DiagnosticHeaders.MessageTypes, string.Join(",", context.SupportedMessageTypes));

            if (context.CorrelationId != null)
                activity.AddBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString());
            if (context.ConversationId != null)
                activity.AddBaggage(DiagnosticHeaders.CorrelationConversationId, context.ConversationId.Value.ToString());

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

            return activity;
        }
    }
}
