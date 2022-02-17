#nullable enable
namespace MassTransit.Logging
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;
    using Transports;


    public readonly struct EnabledActivitySource
    {
        readonly ActivitySource _source;
        readonly string _name;

        public EnabledActivitySource(ActivitySource source, string name)
        {
            _source = source;
            _name = name;
        }

        public StartedActivity? StartSendActivity<T>(SendContext<T> context, params (string Key, object Value)[] tags)
            where T : class
        {
            var parentActivityContext = System.Diagnostics.Activity.Current?.Context ?? default;

            var activity = _source.CreateActivity(_name, ActivityKind.Producer, parentActivityContext);
            if (activity == null)
                return null;

            activity.AddTag(DiagnosticHeaders.Messaging.Destination, _name.Substring(0, _name.Length - 5));

            var conversationId = context.ConversationId?.ToString("D");

            if (context.CorrelationId.HasValue)
                activity.AddBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString("D"));
            if (conversationId != null)
                activity.AddBaggage(DiagnosticHeaders.Messaging.ConversationId, conversationId);

            activity.Start();

            if (activity.IsAllDataRequested)
            {
                if (context.MessageId.HasValue)
                    activity.AddTag(DiagnosticHeaders.MessageId, context.MessageId.Value.ToString("D"));
                if (conversationId != null)
                    activity.AddTag(DiagnosticHeaders.Messaging.ConversationId, conversationId);
                if (context.CorrelationId.HasValue)
                    activity.AddTag(DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString("D"));
                if (context.RequestId.HasValue)
                    activity.AddTag(DiagnosticHeaders.RequestId, context.RequestId.Value.ToString("D"));
                if (context.InitiatorId.HasValue)
                    activity.AddTag(DiagnosticHeaders.InitiatorId, context.InitiatorId.Value.ToString("D"));
                if (context.SourceAddress != null)
                    activity.AddTag(DiagnosticHeaders.SourceAddress, context.SourceAddress.ToString());
                if (context.DestinationAddress != null)
                    activity.AddTag(DiagnosticHeaders.DestinationAddress, context.DestinationAddress.ToString());

                activity.AddTag(DiagnosticHeaders.MessageTypes, string.Join(",", MessageTypeCache<T>.MessageTypeNames));

                for (var i = 0; i < tags.Length; i++)
                {
                    if (tags[i].Value != null)
                        activity.AddTag(tags[i].Key, tags[i].Value?.ToString());
                }
            }

            if (activity.Id != null)
                context.Headers.Set(DiagnosticHeaders.ActivityId, activity.Id);

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

            if (baggage != null)
                context.Headers.Set(DiagnosticHeaders.ActivityCorrelationContext, baggage);

            EnabledScope? scope = BeginScope(activity);

            return new StartedActivity(activity, scope);
        }

        public StartedActivity? StartReceiveActivity(ReceiveContext context, params (string Key, string Value)[] tags)
        {
            var parentActivityContext = GetParentActivityContext(context.TransportHeaders);
            var activity = _source.CreateActivity(_name, ActivityKind.Consumer, parentActivityContext);
            if (activity == null)
                return null;

            activity.Start();

            if (activity.IsAllDataRequested)
            {
                activity.AddTag(DiagnosticHeaders.Messaging.Destination, context.InputAddress.GetEndpointName());
                activity.AddTag(DiagnosticHeaders.Messaging.Operation, "receive");
                activity.AddTag(DiagnosticHeaders.InputAddress, context.InputAddress.ToString());

                if ((context.TransportHeaders.TryGetHeader(MessageHeaders.TransportMessageId, out var messageIdHeader)
                        || context.TransportHeaders.TryGetHeader(MessageHeaders.MessageId, out messageIdHeader))
                    && messageIdHeader is string text)
                    activity.AddTag(DiagnosticHeaders.Messaging.TransportMessageId, text);

                for (var i = 0; i < tags.Length; i++)
                {
                    if (tags[i].Value != null)
                        activity.AddTag(tags[i].Key, tags[i].Value);
                }
            }

            EnabledScope? scope = BeginScope(activity);

            return new StartedActivity(activity, scope);
        }

        public StartedActivity? StartConsumerActivity<TConsumer, T>(ConsumeContext<T> context)
            where T : class
        {
            return StartActivity(activity =>
            {
                activity.AddTag(DiagnosticHeaders.ServiceName, TypeCache<TConsumer>.ShortName);
                activity.AddTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<T>.DiagnosticAddress);
            });
        }

        public StartedActivity? StartHandlerActivity<T>(ConsumeContext<T> context)
            where T : class
        {
            return StartActivity(activity =>
            {
                activity.AddTag(DiagnosticHeaders.ServiceName, "Handler");
                activity.AddTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<T>.DiagnosticAddress);
            });
        }

        public StartedActivity? StartSagaActivity<TSaga, T>(SagaConsumeContext<TSaga, T> context)
            where TSaga : class, ISaga
            where T : class
        {
            return StartActivity(activity =>
            {
                activity.AddTag(DiagnosticHeaders.SagaId, context.Saga.CorrelationId.ToString("D"));
                activity.AddTag(DiagnosticHeaders.ServiceName, TypeCache<TSaga>.ShortName);
                activity.AddTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<T>.DiagnosticAddress);
            });
        }

        public async ValueTask<StartedActivity?> StartSagaStateMachineActivity<TSaga, T>(BehaviorContext<TSaga, T> context)
            where TSaga : class, ISaga
            where T : class
        {
            State<TSaga>? beginState = await context.StateMachine.Accessor.Get(context).ConfigureAwait(false);

            return StartActivity(activity =>
            {
                activity.AddTag(DiagnosticHeaders.SagaId, context.Saga.CorrelationId.ToString("D"));
                activity.AddTag(DiagnosticHeaders.ServiceName, context.StateMachine.Name);
                activity.AddTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<T>.DiagnosticAddress);

                if (beginState != null)
                    activity.AddTag(DiagnosticHeaders.BeginState, beginState.Name);
            });
        }

        public StartedActivity? StartExecuteActivity<TActivity, TArguments>(ConsumeContext<RoutingSlip> context)
            where TActivity : IExecuteActivity<TArguments>
            where TArguments : class
        {
            return StartActivity(activity =>
            {
                activity.AddTag(DiagnosticHeaders.TrackingNumber, context.Message.TrackingNumber.ToString("D"));
                activity.AddTag(DiagnosticHeaders.ServiceName, TypeCache<TActivity>.ShortName);
                activity.AddTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<TArguments>.DiagnosticAddress);
            });
        }

        public StartedActivity? StartCompensateActivity<TActivity, TLog>(ConsumeContext<RoutingSlip> context)
            where TActivity : ICompensateActivity<TLog>
            where TLog : class
        {
            return StartActivity(activity =>
            {
                activity.AddTag(DiagnosticHeaders.TrackingNumber, context.Message.TrackingNumber.ToString("D"));
                activity.AddTag(DiagnosticHeaders.ServiceName, TypeCache<TActivity>.ShortName);
                activity.AddTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<TLog>.DiagnosticAddress);
            });
        }

        StartedActivity? StartActivity(Action<System.Diagnostics.Activity> started)
        {
            var operationName = _name;

            var currentActivity = System.Diagnostics.Activity.Current;
            if (currentActivity != null)
            {
                operationName = Cached.OperationNames.GetOrAdd(currentActivity.OperationName, add =>
                {
                    if (add.EndsWith(" receive"))
                        return add.Substring(0, add.Length - 8) + " process";
                    if (add.EndsWith(" process"))
                        return add;

                    return operationName;
                });
            }

            var activity = _source.CreateActivity(operationName, ActivityKind.Consumer, currentActivity?.Context ?? default);
            if (activity == null)
                return null;

            activity.Start();

            activity.AddTag(DiagnosticHeaders.Messaging.Operation, "process");

            if (activity.IsAllDataRequested)
                started(activity);

            EnabledScope? scope = BeginScope(activity);

            return new StartedActivity(activity, scope);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<string, string> OperationNames = new ConcurrentDictionary<string, string>();
        }


        static ActivityContext GetParentActivityContext(Headers headers)
        {
            return headers.TryGetHeader(DiagnosticHeaders.ActivityId, out var headerValue) && headerValue is string activityId
                && ActivityContext.TryParse(activityId, null, out var activityContext)
                    ? activityContext
                    : default;
        }

        static EnabledScope? BeginScope(System.Diagnostics.Activity activity)
        {
            EnabledScope? scope = LogContext.BeginScope();
            if (!scope.HasValue)
                return scope;

            var spanId = activity.GetSpanId();
            if (!string.IsNullOrWhiteSpace(spanId))
                scope.Value.Add("SpanId", spanId);

            var traceId = activity.GetTraceId();
            if (!string.IsNullOrWhiteSpace(traceId))
                scope.Value.Add("TraceId", traceId);

            var parentId = activity.GetParentId();
            if (!string.IsNullOrWhiteSpace(parentId))
                scope.Value.Add("ParentId", parentId);

            return scope;
        }
    }


    static class ActivityExtensions
    {
        public static string GetSpanId(this System.Diagnostics.Activity activity)
        {
            return activity.IdFormat switch
            {
                ActivityIdFormat.Hierarchical => activity.Id,
                ActivityIdFormat.W3C => activity.SpanId.ToHexString(),
                _ => null,
            } ?? string.Empty;
        }

        public static string GetTraceId(this System.Diagnostics.Activity activity)
        {
            return activity.IdFormat switch
            {
                ActivityIdFormat.Hierarchical => activity.RootId,
                ActivityIdFormat.W3C => activity.TraceId.ToHexString(),
                _ => null,
            } ?? string.Empty;
        }

        public static string GetParentId(this System.Diagnostics.Activity activity)
        {
            return activity.IdFormat switch
            {
                ActivityIdFormat.Hierarchical => activity.ParentId,
                ActivityIdFormat.W3C => activity.ParentSpanId.ToHexString(),
                _ => null,
            } ?? string.Empty;
        }
    }
}
