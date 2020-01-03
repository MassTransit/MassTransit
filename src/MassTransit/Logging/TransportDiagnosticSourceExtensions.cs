namespace MassTransit.Logging
{
    using System.Collections.Generic;
    using System.Linq;
    using Context;


    public static class TransportDiagnosticSourceExtensions
    {
        public static StartedActivity? StartSendActivity<T>(this EnabledDiagnosticSource source, SendContext<T> context, params (string, string)[] tags)
            where T : class
        {
            var startedActivity = source.StartActivity(GetSendBags(context), GetSendTags(context, tags));

            var activity = startedActivity.Value;

            context.Headers.Set(DiagnosticHeaders.ActivityId, activity.Activity.Id);

            if (activity.Activity.Baggage.Any())
            {
                var bags = CleanUpBaggage(activity.Activity.Baggage).ToList();
                if (bags.Count > 0)
                {
                    context.Headers.Set(DiagnosticHeaders.ActivityCorrelationContext, bags);
                }
            }

            return startedActivity;
        }

        public static StartedActivity? StartReceiveActivity(this EnabledDiagnosticSource source, ReceiveContext context, params (string, string)[] tags)
        {
            string parentActivityId = null;
            if (context.TransportHeaders.TryGetHeader(DiagnosticHeaders.ActivityId, out var headerValue)
                && headerValue is string activityId && !string.IsNullOrWhiteSpace(activityId))
            {
                parentActivityId = activityId;
            }

            var startedActivity = source.StartActivity(null, GetReceiveTags(context, tags), parentActivityId);

            var activity = startedActivity.Value;

            context.AddOrUpdatePayload<StartedActivityContext>(() => activity, _ => activity);

            return startedActivity;
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

        static IEnumerable<KeyValuePair<string, string>> CleanUpBaggage(IEnumerable<KeyValuePair<string, string>> baggage)
        {
            foreach (var pair in baggage)
            {
                switch (pair.Key)
                {
                    case DiagnosticHeaders.ConversationId:
                    case DiagnosticHeaders.CorrelationId:
                        break;
                    default:
                        yield return pair;
                        break;
                }
            }
        }

        static IEnumerable<(string, string)> GetSendBags(this SendContext context)
        {
            if (context.CorrelationId.HasValue)
                yield return (DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString("D"));
            if (context.ConversationId.HasValue)
                yield return (DiagnosticHeaders.ConversationId, context.ConversationId.Value.ToString("D"));
        }

        static IEnumerable<(string, string)> GetReceiveTags(this ReceiveContext context, IEnumerable<(string, string)> tags)
        {
            yield return (DiagnosticHeaders.ServiceKind, DiagnosticHeaders.Kind.Consumer);
            yield return (DiagnosticHeaders.PeerHost, context.InputAddress.Host);
            yield return (DiagnosticHeaders.PeerAddress, context.InputAddress.AbsolutePath);
            yield return (DiagnosticHeaders.PeerService, "Receive");

            if (context.TransportHeaders.TryGetHeader("MessageId", out var messageIdHeader) && messageIdHeader != null)
                yield return (DiagnosticHeaders.MessageId, messageIdHeader.ToString());

            yield return (DiagnosticHeaders.InputAddress, context.InputAddress.ToString());

            foreach (var (key, value) in tags)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    yield return (key, value);
            }
        }

        static IEnumerable<(string, string)> GetSendTags(this SendContext context, IEnumerable<(string, string)> tags)
        {
            yield return (DiagnosticHeaders.ServiceKind, DiagnosticHeaders.Kind.Producer);
            yield return (DiagnosticHeaders.PeerHost, context.DestinationAddress.Host);
            yield return (DiagnosticHeaders.PeerAddress, context.DestinationAddress.AbsolutePath);
            yield return (DiagnosticHeaders.PeerService, "Send");

            if (context.MessageId.HasValue)
                yield return (DiagnosticHeaders.MessageId, context.MessageId.Value.ToString("D"));
            if (context.ConversationId.HasValue)
                yield return (DiagnosticHeaders.ConversationId, context.ConversationId.Value.ToString("D"));
            if (context.CorrelationId.HasValue)
                yield return (DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString("D"));
            if (context.InitiatorId.HasValue)
                yield return (DiagnosticHeaders.InitiatorId, context.InitiatorId.Value.ToString("D"));
            if (context.SourceAddress != null)
                yield return (DiagnosticHeaders.SourceAddress, context.SourceAddress.ToString());
            if (context.DestinationAddress != null)
                yield return (DiagnosticHeaders.DestinationAddress, context.DestinationAddress.ToString());

            foreach (var (key, value) in tags)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    yield return (key, value);
            }
        }
    }
}
