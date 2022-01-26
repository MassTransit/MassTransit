#nullable enable
namespace MassTransit.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;


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

        public static void AddConsumeContextTags(this Activity activity, ConsumeContext context)
        {
            AddTag(activity, DiagnosticHeaders.Messaging.MessageId, context.MessageId);
            AddTag(activity, DiagnosticHeaders.Messaging.ConversationId, context.ConversationId);

            AddTag(activity, DiagnosticHeaders.CorrelationId, context.CorrelationId);
            AddTag(activity, DiagnosticHeaders.InitiatorId, context.InitiatorId);
            AddTag(activity, DiagnosticHeaders.SourceAddress, context.SourceAddress);
            AddTag(activity, DiagnosticHeaders.DestinationAddress, context.DestinationAddress);

            if (context.Host != null)
                activity.AddTag(DiagnosticHeaders.SourceHostMachine, context.Host.MachineName);

            if (context.SupportedMessageTypes != null)
                activity.AddTag(DiagnosticHeaders.MessageTypes, string.Join(",", context.SupportedMessageTypes));

            if (context.CorrelationId.HasValue)
                activity.AddBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString("D"));
            if (context.ConversationId.HasValue)
                activity.AddBaggage(DiagnosticHeaders.Messaging.ConversationId, context.ConversationId.Value.ToString("D"));

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
