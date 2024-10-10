#nullable enable
// ReSharper disable once CheckNamespace
namespace MassTransit.Logging
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Courier.Contracts;
    using Metadata;
    using Middleware;
    using Transports;


    public static class LogContextActivityExtensions
    {
        public static StartedActivity? StartSendActivity<T>(this ILogContext logContext, SendTransportContext transportContext, SendContext<T> context,
            params (string Key, object? Value)[] tags)
            where T : class
        {
            var parentActivityContext = System.Diagnostics.Activity.Current == null
                ? GetParentActivityContext(context.Headers)
                : default;

            var activity = Cached.Source.Value.CreateActivity(transportContext.ActivityName, ActivityKind.Producer, parentActivityContext);
            if (activity == null)
                return null;

            activity.SetTag(DiagnosticHeaders.Messaging.Operation, "send");
            activity.SetTag(DiagnosticHeaders.Messaging.System, transportContext.ActivitySystem);
            activity.SetTag(DiagnosticHeaders.Messaging.DestinationName, transportContext.ActivityDestination);

            return PopulateSendActivity<T>(context, activity, tags);
        }

        public static StartedActivity? StartOutboxSendActivity<T>(this ILogContext logContext, SendContext<T> context)
            where T : class
        {
            var parentActivityContext = System.Diagnostics.Activity.Current == null
                ? GetParentActivityContext(context.Headers)
                : default;

            var activity = Cached.Source.Value.CreateActivity("outbox send", ActivityKind.Producer, parentActivityContext);
            if (activity == null)
                return null;

            activity.SetTag(DiagnosticHeaders.Messaging.Operation, "send");

            return PopulateSendActivity<T>(context, activity);
        }

        public static StartedActivity? StartOutboxDeliverActivity(this ILogContext logContext, OutboxMessageContext context)
        {
            var parentActivityContext = GetParentActivityContext(context.Headers);

            var activity = Cached.Source.Value.CreateActivity("outbox process", ActivityKind.Client, parentActivityContext);
            if (activity == null)
                return null;

            activity.Start();

            return new StartedActivity(activity);
        }

        public static StartedActivity? StartReceiveActivity(this ILogContext logContext, string name, string inputAddress, string endpointName,
            ReceiveContext context)
        {
            var parentActivityContext = GetParentActivityContext(context.TransportHeaders, true);

            var activity = context.TransportHeaders.TryGetHeader(DiagnosticHeaders.ActivityPropagation, out var linkTypeValue) switch
            {
                true => linkTypeValue switch
                {
                    "Link" => Cached.Source.Value.CreateActivity(name, ActivityKind.Consumer, (ActivityContext)default,
                        links: [new ActivityLink(parentActivityContext)]),
                    "New" => Cached.Source.Value.CreateActivity(name, ActivityKind.Consumer, (ActivityContext)default),
                    _ => Cached.Source.Value.CreateActivity(name, ActivityKind.Consumer, parentActivityContext)
                },
                false => Cached.Source.Value.CreateActivity(name, ActivityKind.Consumer, parentActivityContext)
            };

            if (activity == null)
                return null;

            activity.SetTag(DiagnosticHeaders.Messaging.Operation, "receive");
            activity.SetTag(DiagnosticHeaders.Messaging.DestinationName, endpointName);

            if (activity.IsAllDataRequested)
            {
                activity.SetTag(DiagnosticHeaders.InputAddress, inputAddress);

                if ((context.TransportHeaders.TryGetHeader(MessageHeaders.TransportMessageId, out var messageIdHeader)
                        || context.TransportHeaders.TryGetHeader(MessageHeaders.MessageId, out messageIdHeader))
                    && messageIdHeader is string text)
                    activity.SetTag(DiagnosticHeaders.Messaging.TransportMessageId, text);
            }

            activity.Start();

            return new StartedActivity(activity);
        }

        public static StartedActivity? StartConsumerActivity<TConsumer, T>(this ILogContext logContext, ConsumeContext<T> context)
            where T : class
        {
            return StartActivity(context, activity =>
            {
                activity.SetTag(DiagnosticHeaders.ConsumerType, TypeCache<TConsumer>.ShortName);
                activity.SetTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<T>.DiagnosticAddress);
            });
        }

        public static StartedActivity? StartHandlerActivity<T>(this ILogContext logContext, ConsumeContext<T> context)
            where T : class
        {
            return StartActivity(context, activity =>
            {
                activity.SetTag(DiagnosticHeaders.ConsumerType, "Handler");
                activity.SetTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<T>.DiagnosticAddress);
            });
        }

        public static StartedActivity? StartSagaActivity<TSaga, T>(this ILogContext logContext, SagaConsumeContext<TSaga, T> context)
            where TSaga : class, ISaga
            where T : class
        {
            return StartActivity(context, activity =>
            {
                activity.SetTag(DiagnosticHeaders.SagaId, context.Saga.CorrelationId.ToString("D"));
                activity.SetTag(DiagnosticHeaders.ConsumerType, TypeCache<TSaga>.ShortName);
                activity.SetTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<T>.DiagnosticAddress);
            });
        }

        public static StartedActivity? StartSagaStateMachineActivity<TSaga, T>(this ILogContext logContext, BehaviorContext<TSaga, T> context)
            where TSaga : class, SagaStateMachineInstance
            where T : class
        {
            return StartActivity(context, activity =>
            {
                activity.SetTag(DiagnosticHeaders.SagaId, context.Saga.CorrelationId.ToString("D"));
                activity.SetTag(DiagnosticHeaders.ConsumerType, context.StateMachine.Name);
                activity.SetTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<T>.DiagnosticAddress);
            });
        }

        public static StartedActivity? StartExecuteActivity<TActivity, TArguments>(this ILogContext logContext, ConsumeContext<RoutingSlip> context)
            where TActivity : IExecuteActivity<TArguments>
            where TArguments : class
        {
            return StartActivity(context, activity =>
            {
                activity.SetTag(DiagnosticHeaders.TrackingNumber, context.Message.TrackingNumber.ToString("D"));
                activity.SetTag(DiagnosticHeaders.ConsumerType, TypeCache<TActivity>.ShortName);
                activity.SetTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<TArguments>.DiagnosticAddress);
            });
        }

        public static StartedActivity? StartCompensateActivity<TActivity, TLog>(this ILogContext logContext, ConsumeContext<RoutingSlip> context)
            where TActivity : ICompensateActivity<TLog>
            where TLog : class
        {
            return StartActivity(context, activity =>
            {
                activity.SetTag(DiagnosticHeaders.TrackingNumber, context.Message.TrackingNumber.ToString("D"));
                activity.SetTag(DiagnosticHeaders.ConsumerType, TypeCache<TActivity>.ShortName);
                activity.SetTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<TLog>.DiagnosticAddress);
            });
        }

        public static StartedActivity? StartGenericActivity(this ILogContext logContext, string operationName)
        {
            var activity = Cached.Source.Value.CreateActivity(operationName, ActivityKind.Client);
            if (activity == null)
                return null;

            activity.Start();

            return new StartedActivity(activity);
        }

        static StartedActivity? PopulateSendActivity<T>(SendContext context, System.Diagnostics.Activity activity, params (string Key, object? Value)[] tags)
            where T : class
        {
            var conversationId = context.ConversationId?.ToString("D");

            if (context.CorrelationId.HasValue)
                activity.SetBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString("D"));
            if (conversationId != null)
                activity.SetBaggage(DiagnosticHeaders.Messaging.ConversationId, conversationId);

            if (activity.IsAllDataRequested)
            {
                if (context.MessageId.HasValue)
                    activity.SetTag(DiagnosticHeaders.MessageId, context.MessageId.Value.ToString("D"));
                if (conversationId != null)
                    activity.SetTag(DiagnosticHeaders.Messaging.ConversationId, conversationId);
                if (context.CorrelationId.HasValue)
                    activity.SetTag(DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString("D"));
                if (context.RequestId.HasValue)
                    activity.SetTag(DiagnosticHeaders.RequestId, context.RequestId.Value.ToString("D"));
                if (context.InitiatorId.HasValue)
                    activity.SetTag(DiagnosticHeaders.InitiatorId, context.InitiatorId.Value.ToString("D"));
                if (context.SourceAddress != null)
                    activity.SetTag(DiagnosticHeaders.SourceAddress, context.SourceAddress.ToString());
                if (context.DestinationAddress != null)
                    activity.SetTag(DiagnosticHeaders.DestinationAddress, context.DestinationAddress.ToString());

                activity.SetTag(DiagnosticHeaders.MessageTypes, string.Join(",", context.SupportedMessageTypes));

                for (var i = 0; i < tags.Length; i++)
                {
                    if (tags[i].Value != null)
                        activity.SetTag(tags[i].Key, tags[i].Value?.ToString());
                }
            }

            activity.Start();

            IList<KeyValuePair<string, string?>>? baggage = null;
            foreach (KeyValuePair<string, string?> pair in activity.Baggage)
            {
                if (pair.Key.Equals(DiagnosticHeaders.Messaging.ConversationId) || pair.Key.Equals(DiagnosticHeaders.CorrelationId))
                    continue;

                if (string.IsNullOrWhiteSpace(pair.Value))
                    continue;

                baggage ??= new List<KeyValuePair<string, string?>>();
                baggage.Add(pair);
            }

            if (activity.Id != null)
                context.Headers.Set(DiagnosticHeaders.ActivityId, activity.Id);

            if (baggage != null)
                context.Headers.Set(DiagnosticHeaders.ActivityCorrelationContext, baggage);

            return new StartedActivity(activity);
        }

        static ActivityContext GetParentActivityContext(Headers headers, bool isRemote = false)
        {
            if (headers.TryGetHeader(DiagnosticHeaders.ActivityId, out var headerValue)
                && headerValue is string activityId
                && ActivityContext.TryParse(activityId, null, out var activityContext))
            {
                if (isRemote && System.Diagnostics.Activity.Current == null)
                    return new ActivityContext(activityContext.TraceId, activityContext.SpanId, activityContext.TraceFlags, activityContext.TraceState, true);

                return activityContext;
            }

            return default;
        }

        static StartedActivity? StartActivity(ConsumeContext context, Action<System.Diagnostics.Activity> started)
        {
            var currentActivity = System.Diagnostics.Activity.Current;
            if (currentActivity == null)
                return null;

            var operationName = Cached.OperationNames.GetOrAdd(currentActivity.OperationName, add =>
            {
                if (add.EndsWith(" receive"))
                    return add.Substring(0, add.Length - 8) + " process";
                if (add.EndsWith(" process"))
                    return add;

                return currentActivity.OperationName;
            });

            var activity = Cached.Source.Value.CreateActivity(operationName, ActivityKind.Consumer);
            if (activity == null)
                return null;

            activity.SetTag(DiagnosticHeaders.Messaging.Operation, "process");

            if (activity.IsAllDataRequested)
            {
                if (context.MessageId.HasValue)
                    activity.SetTag(DiagnosticHeaders.MessageId, context.MessageId.Value.ToString("D"));
                if (context.ConversationId.HasValue)
                    activity.SetTag(DiagnosticHeaders.Messaging.ConversationId, context.ConversationId.Value.ToString("D"));
                if (context.CorrelationId.HasValue)
                    activity.SetTag(DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString("D"));
                if (context.RequestId.HasValue)
                    activity.SetTag(DiagnosticHeaders.RequestId, context.RequestId.Value.ToString("D"));
                if (context.InitiatorId.HasValue)
                    activity.SetTag(DiagnosticHeaders.InitiatorId, context.InitiatorId.Value.ToString("D"));
                if (context.SourceAddress != null)
                    activity.SetTag(DiagnosticHeaders.SourceAddress, context.SourceAddress.ToString());
                if (context.DestinationAddress != null)
                    activity.SetTag(DiagnosticHeaders.DestinationAddress, context.DestinationAddress.ToString());

                activity.SetTag(DiagnosticHeaders.MessageTypes, string.Join(",", context.SupportedMessageTypes));

                started(activity);
            }

            activity.Start();

            return new StartedActivity(activity);
        }


        static class Cached
        {
            internal static readonly Lazy<ActivitySource> Source = new Lazy<ActivitySource>(() =>
                new ActivitySource(DiagnosticHeaders.DefaultListenerName, HostMetadataCache.Host.MassTransitVersion));

            internal static readonly ConcurrentDictionary<string, string> OperationNames = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
        }
    }
}
