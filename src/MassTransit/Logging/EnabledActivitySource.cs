#nullable enable
namespace MassTransit.Logging
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;
    using Metadata;
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
            var initialTags = new ActivityTagsCollection { [DiagnosticHeaders.Messaging.Destination] = context.DestinationAddress.GetDiagnosticEndpointName() };

            if (context.MessageId.HasValue)
                initialTags[DiagnosticHeaders.Messaging.MessageId] = context.MessageId.Value.ToString("D");

            var conversationId = context.ConversationId?.ToString("D");
            if (conversationId != null)
                initialTags[DiagnosticHeaders.Messaging.ConversationId] = conversationId;

            if (context.CorrelationId.HasValue)
                initialTags[DiagnosticHeaders.CorrelationId] = context.CorrelationId.Value.ToString("D");
            if (context.InitiatorId.HasValue)
                initialTags[DiagnosticHeaders.InitiatorId] = context.InitiatorId.Value.ToString("D");
            if (context.SourceAddress != null)
                initialTags[DiagnosticHeaders.SourceAddress] = context.SourceAddress.ToString();
            if (context.DestinationAddress != null)
                initialTags[DiagnosticHeaders.DestinationAddress] = context.DestinationAddress.ToString();

            for (var i = 0; i < tags.Length; i++)
                initialTags[tags[i].Key] = tags[i].Value;

            var parentActivityContext = System.Diagnostics.Activity.Current?.Context ?? default;

            var activity = _source.CreateActivity(_name, ActivityKind.Producer, parentActivityContext, initialTags);
            if (activity == null)
                return null;

            if (context.CorrelationId.HasValue)
                activity.AddBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString("D"));
            if (conversationId != null)
                activity.AddBaggage(DiagnosticHeaders.Messaging.ConversationId, conversationId);

            activity.Start();

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

            var initialTags = new ActivityTagsCollection
            {
                [DiagnosticHeaders.Messaging.Destination] = context.InputAddress.GetEndpointName(),
                [DiagnosticHeaders.Messaging.Operation] = "receive",
                [DiagnosticHeaders.InputAddress] = context.InputAddress.ToString()
            };

            if (context.TransportHeaders.TryGetHeader(MessageHeaders.MessageId, out var messageIdHeader) && messageIdHeader is string text)
                initialTags[DiagnosticHeaders.Messaging.MessageId] = text;

            for (var i = 0; i < tags.Length; i++)
                initialTags[tags[i].Key] = tags[i].Value;

            var activity = _source.CreateActivity(_name, ActivityKind.Consumer, parentActivityContext, initialTags);
            if (activity == null)
                return null;

            activity.Start();

            EnabledScope? scope = BeginScope(activity);

            return new StartedActivity(activity, scope);
        }

        public StartedActivity? StartConsumerActivity<TConsumer, T>(ConsumeContext<T> context)
            where T : class
        {
            var initialTags = new ActivityTagsCollection
            {
                [DiagnosticHeaders.ServiceName] = TypeCache<TConsumer>.ShortName,
                [DiagnosticHeaders.PeerService] = "Consumer",
                [DiagnosticHeaders.PeerAddress] = MessageTypeCache<T>.DiagnosticAddress,
            };

            return StartActivity(context, initialTags);
        }

        public StartedActivity? StartHandlerActivity<T>(ConsumeContext<T> context)
            where T : class
        {
            var initialTags = new ActivityTagsCollection
            {
                [DiagnosticHeaders.PeerService] = "Handler",
                [DiagnosticHeaders.PeerAddress] = MessageTypeCache<T>.DiagnosticAddress,
            };

            return StartActivity(context, initialTags);
        }

        public StartedActivity? StartSagaActivity<TSaga, T>(ConsumeContext<T> context)
            where TSaga : ISaga
            where T : class
        {
            var initialTags = new ActivityTagsCollection
            {
                [DiagnosticHeaders.PeerService] = "Repository",
                [DiagnosticHeaders.PeerAddress] = MessageTypeCache<T>.DiagnosticAddress,
            };

            return StartActivity(context, initialTags);
        }

        public StartedActivity? StartSagaActivity<TSaga, T>(SagaConsumeContext<TSaga, T> context)
            where TSaga : class, ISaga
            where T : class
        {
            var initialTags = new ActivityTagsCollection
            {
                [DiagnosticHeaders.SagaId] = context.Saga.CorrelationId.ToString("D"),
                [DiagnosticHeaders.PeerService] = TypeCache<TSaga>.ShortName,
                [DiagnosticHeaders.PeerAddress] = MessageTypeCache<T>.DiagnosticAddress,
            };

            return StartActivity(context, initialTags);
        }

        public async ValueTask<StartedActivity?> StartSagaStateMachineActivity<TSaga, T>(BehaviorContext<TSaga, T> context)
            where TSaga : class, ISaga
            where T : class
        {
            State<TSaga>? beginState = await context.StateMachine.Accessor.Get(context).ConfigureAwait(false);

            var initialTags = new ActivityTagsCollection
            {
                [DiagnosticHeaders.SagaId] = context.Saga.CorrelationId.ToString("D"),
                [DiagnosticHeaders.ServiceName] = TypeCache<TSaga>.ShortName,
                [DiagnosticHeaders.PeerAddress] = MessageTypeCache<T>.DiagnosticAddress,
            };

            if (beginState != null)
                initialTags[DiagnosticHeaders.BeginState] = beginState.Name;

            return StartActivity(context, initialTags);
        }

        public StartedActivity? StartExecuteActivity<TActivity, TArguments>(ConsumeContext<RoutingSlip> context)
            where TActivity : IExecuteActivity<TArguments>
            where TArguments : class
        {
            var initialTags = new ActivityTagsCollection
            {
                [DiagnosticHeaders.TrackingNumber] = context.Message.TrackingNumber.ToString("D"),
                [DiagnosticHeaders.ServiceName] = TypeCache<TActivity>.ShortName,
                [DiagnosticHeaders.PeerAddress] = MessageTypeCache<TArguments>.DiagnosticAddress,
            };

            return StartActivity(context, initialTags);
        }

        public StartedActivity? StartCompensateActivity<TActivity, TLog>(ConsumeContext<RoutingSlip> context)
            where TActivity : ICompensateActivity<TLog>
            where TLog : class
        {
            var initialTags = new ActivityTagsCollection
            {
                [DiagnosticHeaders.TrackingNumber] = context.Message.TrackingNumber.ToString("D"),
                [DiagnosticHeaders.ServiceName] = TypeCache<TActivity>.ShortName,
                [DiagnosticHeaders.PeerAddress] = MessageTypeCache<TLog>.DiagnosticAddress,
            };

            return StartActivity(context, initialTags);
        }

        StartedActivity? StartActivity(ConsumeContext context, ActivityTagsCollection initialTags)
        {
            var operationName = _name;

            var currentActivity = System.Diagnostics.Activity.Current;
            if (currentActivity != null)
            {
                if (currentActivity.OperationName.EndsWith(" receive"))
                    operationName = currentActivity.OperationName.Substring(0, currentActivity.OperationName.Length - 8) + " process";

                foreach (KeyValuePair<string, string?> tag in currentActivity.Tags)
                {
                    switch (tag.Key)
                    {
                        case DiagnosticHeaders.Messaging.Destination:
                            initialTags[DiagnosticHeaders.Messaging.Destination] = tag.Value;
                            break;
                    }
                }
            }
            else
                initialTags[DiagnosticHeaders.Messaging.Destination] = context.ReceiveContext.InputAddress.GetDiagnosticEndpointName();

            initialTags[DiagnosticHeaders.Messaging.Operation] = "process";

            initialTags[DiagnosticHeaders.PeerHost] = HostMetadataCache.Host.MachineName;

            var activity = _source.CreateActivity(operationName, ActivityKind.Consumer, currentActivity?.Context ?? default, initialTags);
            if (activity == null)
                return null;

            activity.Start();

            EnabledScope? scope = BeginScope(activity);

            return new StartedActivity(activity, scope);
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
            if (scope.HasValue)
            {
                var spanId = activity.GetSpanId();
                if (!string.IsNullOrWhiteSpace(spanId))
                    scope.Value.Add("SpanId", spanId);

                var traceId = activity.GetTraceId();
                if (!string.IsNullOrWhiteSpace(traceId))
                    scope.Value.Add("TraceId", traceId);

                var parentId = activity.GetParentId();
                if (!string.IsNullOrWhiteSpace(parentId))
                    scope.Value.Add("ParentId", parentId);
            }

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
