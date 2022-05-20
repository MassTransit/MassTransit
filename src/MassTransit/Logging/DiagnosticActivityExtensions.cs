#nullable enable
namespace MassTransit.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;


    public static class DiagnosticActivityExtensions
    {
        static void AddTag(Activity activity, string key, Guid? value)
        {
            if (value.HasValue)
                activity.AddTag(key, value.Value.ToString("D"));
        }

        static void AddTag(Activity activity, string key, Uri? value)
        {
            if (value != null)
                activity.AddTag(key, value.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddConsumeContextTags(this Activity activity, ConsumeContext context)
        {
            if (activity.IsAllDataRequested)
            {
                AddTag(activity, DiagnosticHeaders.MessageId, context.MessageId);
                AddTag(activity, DiagnosticHeaders.Messaging.ConversationId, context.ConversationId);

                AddTag(activity, DiagnosticHeaders.CorrelationId, context.CorrelationId);
                AddTag(activity, DiagnosticHeaders.InitiatorId, context.InitiatorId);
                AddTag(activity, DiagnosticHeaders.RequestId, context.RequestId);
                AddTag(activity, DiagnosticHeaders.SourceAddress, context.SourceAddress);
                AddTag(activity, DiagnosticHeaders.DestinationAddress, context.DestinationAddress);

                activity.AddTag(DiagnosticHeaders.MessageTypes, string.Join(",", context.SupportedMessageTypes));
            }

            if (context.CorrelationId.HasValue)
                activity.AddBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString("D"));
            if (context.ConversationId.HasValue)
                activity.AddBaggage(DiagnosticHeaders.Messaging.ConversationId, context.ConversationId.Value.ToString("D"));

            if (context.TryGetHeader(DiagnosticHeaders.ActivityCorrelationContext, out IEnumerable<KeyValuePair<string, object>>? correlationHeader))
            {
                foreach (KeyValuePair<string, object> value in correlationHeader!)
                {
                    if (value.Value is string text && !string.IsNullOrWhiteSpace(text))
                        activity.AddBaggage(value.Key, text);
                }
            }
        }
    }
}
