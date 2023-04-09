#nullable enable
namespace MassTransit.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;


    public static class DiagnosticActivityExtensions
    {
        static void SetTag(Activity activity, string key, Guid? value)
        {
            if (value.HasValue)
                activity.SetTag(key, value.Value.ToString("D"));
        }

        static void SetTag(Activity activity, string key, Uri? value)
        {
            if (value != null)
                activity.SetTag(key, value.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddConsumeContextTags(this Activity activity, ConsumeContext context)
        {
            if (activity.IsAllDataRequested)
            {
                SetTag(activity, DiagnosticHeaders.MessageId, context.MessageId);
                SetTag(activity, DiagnosticHeaders.Messaging.ConversationId, context.ConversationId);

                SetTag(activity, DiagnosticHeaders.CorrelationId, context.CorrelationId);
                SetTag(activity, DiagnosticHeaders.InitiatorId, context.InitiatorId);
                SetTag(activity, DiagnosticHeaders.RequestId, context.RequestId);
                SetTag(activity, DiagnosticHeaders.SourceAddress, context.SourceAddress);
                SetTag(activity, DiagnosticHeaders.DestinationAddress, context.DestinationAddress);

                activity.SetTag(DiagnosticHeaders.MessageTypes, string.Join(",", context.SupportedMessageTypes));
            }

            if (context.CorrelationId.HasValue)
                activity.SetBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString("D"));
            if (context.ConversationId.HasValue)
                activity.SetBaggage(DiagnosticHeaders.Messaging.ConversationId, context.ConversationId.Value.ToString("D"));

            if (context.TryGetHeader(DiagnosticHeaders.ActivityCorrelationContext, out IEnumerable<KeyValuePair<string, object>>? correlationHeader))
            {
                foreach (KeyValuePair<string, object> value in correlationHeader!)
                {
                    if (value.Value is string text && !string.IsNullOrWhiteSpace(text))
                        activity.SetBaggage(value.Key, text);
                }
            }
        }
    }
}
