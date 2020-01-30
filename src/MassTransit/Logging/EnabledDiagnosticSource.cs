namespace MassTransit.Logging
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Context;
    using Courier;
    using Courier.Contracts;
    using Metadata;
    using Saga;


    public readonly struct EnabledDiagnosticSource
    {
        readonly DiagnosticSource _source;
        readonly string _name;

        public EnabledDiagnosticSource(DiagnosticSource source, string name)
        {
            _source = source;
            _name = name;
        }

        public StartedActivity? StartSendActivity<T>(SendContext<T> context, params (string Key, string Value)[] tags)
            where T : class
        {
            var activity = CreateActivity(context.Headers);

            if (context.CorrelationId.HasValue)
                activity.AddBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString("D"));
            if (context.ConversationId.HasValue)
                activity.AddBaggage(DiagnosticHeaders.ConversationId, context.ConversationId.Value.ToString("D"));

            activity.AddTag(DiagnosticHeaders.ServiceKind, DiagnosticHeaders.Kind.Producer);
            activity.AddTag(DiagnosticHeaders.PeerHost, context.DestinationAddress.Host);
            activity.AddTag(DiagnosticHeaders.PeerAddress, context.DestinationAddress.AbsolutePath);
            activity.AddTag(DiagnosticHeaders.PeerService, "Send");

            if (context.MessageId.HasValue)
                activity.AddTag(DiagnosticHeaders.MessageId, context.MessageId.Value.ToString("D"));
            if (context.ConversationId.HasValue)
                activity.AddTag(DiagnosticHeaders.ConversationId, context.ConversationId.Value.ToString("D"));
            if (context.CorrelationId.HasValue)
                activity.AddTag(DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString("D"));
            if (context.InitiatorId.HasValue)
                activity.AddTag(DiagnosticHeaders.InitiatorId, context.InitiatorId.Value.ToString("D"));
            if (context.SourceAddress != null)
                activity.AddTag(DiagnosticHeaders.SourceAddress, context.SourceAddress.ToString());
            if (context.DestinationAddress != null)
                activity.AddTag(DiagnosticHeaders.DestinationAddress, context.DestinationAddress.ToString());

            if (tags != null)
                for (int i = 0; i < tags.Length; i++)
                    activity.AddTag(tags[i].Key, tags[i].Value);

            var startActivity = _source.StartActivity(activity, context);

            context.Headers.Set(DiagnosticHeaders.ActivityId, startActivity.Id);

            IList<KeyValuePair<string, string>> baggage = null;
            foreach (var pair in startActivity.Baggage)
            {
                if (pair.Key.Equals(DiagnosticHeaders.ConversationId) || pair.Key.Equals(DiagnosticHeaders.CorrelationId))
                    continue;

                (baggage ??= new List<KeyValuePair<string, string>>()).Add(pair);
            }

            if (baggage != null)
                context.Headers.Set(DiagnosticHeaders.ActivityCorrelationContext, baggage);

            EnabledScope? scope = LogContext.BeginScope();

            return new StartedActivity(_source, startActivity, scope);
        }

        public StartedActivity? StartReceiveActivity(ReceiveContext context, params (string Key, string Value)[] tags)
        {
            var activity = CreateActivity(context.TransportHeaders);

            activity.AddTag(DiagnosticHeaders.ServiceKind, DiagnosticHeaders.Kind.Consumer);
            activity.AddTag(DiagnosticHeaders.PeerHost, context.InputAddress.Host);
            activity.AddTag(DiagnosticHeaders.PeerAddress, context.InputAddress.AbsolutePath);
            activity.AddTag(DiagnosticHeaders.PeerService, "Receive");

            if (context.TransportHeaders.TryGetHeader(nameof(MessageContext.MessageId), out var messageIdHeader) && messageIdHeader is string text)
                activity.AddTag(DiagnosticHeaders.MessageId, text);

            activity.AddTag(DiagnosticHeaders.InputAddress, context.InputAddress.ToString());

            if (tags != null)
                for (int i = 0; i < tags.Length; i++)
                    activity.AddTag(tags[i].Key, tags[i].Value);

            var startActivity = _source.StartActivity(activity, context);

            EnabledScope? scope = LogContext.BeginScope();

            var receiveActivity = new StartedActivity(_source, startActivity, scope);

            context.AddOrUpdatePayload<StartedActivityContext>(() => receiveActivity, _ => receiveActivity);

            return receiveActivity;
        }

        public StartedActivity? StartConsumerActivity<TConsumer, T>(ConsumeContext<T> context)
            where T : class
        {
            var activity = new System.Diagnostics.Activity(_name);

            activity.AddTag(DiagnosticHeaders.ConsumerType, TypeMetadataCache<TConsumer>.ShortName);
            activity.AddTag(DiagnosticHeaders.PeerService, "Consumer");
            activity.AddTag(DiagnosticHeaders.PeerAddress, TypeMetadataCache<T>.DiagnosticAddress);

            return StartActivity(context, activity);
        }

        public StartedActivity? StartHandlerActivity<T>(ConsumeContext<T> context)
            where T : class
        {
            var activity = new System.Diagnostics.Activity(_name);

            activity.AddTag(DiagnosticHeaders.PeerService, "Handler");
            activity.AddTag(DiagnosticHeaders.PeerAddress, TypeMetadataCache<T>.DiagnosticAddress);

            return StartActivity(context, activity);
        }

        public StartedActivity? StartSagaActivity<TSaga, T>(ConsumeContext<T> context)
            where TSaga : ISaga
            where T : class
        {
            var activity = new System.Diagnostics.Activity(_name);

            activity.AddTag(DiagnosticHeaders.PeerService, "Repository");
            activity.AddTag(DiagnosticHeaders.PeerAddress, TypeMetadataCache<T>.DiagnosticAddress);
            activity.AddTag(DiagnosticHeaders.SagaType, TypeMetadataCache<TSaga>.ShortName);

            return StartActivity(context, activity);
        }

        public StartedActivity? StartSagaActivity<TSaga, T>(SagaConsumeContext<TSaga, T> context, string beginState = null)
            where TSaga : class, ISaga
            where T : class
        {
            var activity = new System.Diagnostics.Activity(_name);

            activity.AddTag(DiagnosticHeaders.PeerService, "Saga");
            activity.AddTag(DiagnosticHeaders.PeerAddress, TypeMetadataCache<T>.DiagnosticAddress);
            activity.AddTag(DiagnosticHeaders.SagaType, TypeMetadataCache<TSaga>.ShortName);
            activity.AddTag(DiagnosticHeaders.SagaId, context.Saga.CorrelationId.ToString("D"));

            if (beginState != null)
                activity.AddTag(DiagnosticHeaders.BeginState, beginState);

            return StartActivity(context, activity);
        }

        public StartedActivity? StartExecuteActivity<TActivity, TArguments>(ConsumeContext<RoutingSlip> context)
            where TActivity : IExecuteActivity<TArguments>
            where TArguments : class
        {
            var activity = new System.Diagnostics.Activity(_name);

            activity.AddBaggage(DiagnosticHeaders.TrackingNumber, context.Message.TrackingNumber.ToString("D"));

            activity.AddTag(DiagnosticHeaders.PeerService, "Execute");
            activity.AddTag(DiagnosticHeaders.PeerAddress, TypeMetadataCache<TActivity>.DiagnosticAddress);
            activity.AddTag(DiagnosticHeaders.ActivityType, TypeMetadataCache<TActivity>.ShortName);
            activity.AddTag(DiagnosticHeaders.ArgumentType, TypeMetadataCache<TArguments>.ShortName);
            activity.AddTag(DiagnosticHeaders.TrackingNumber, context.Message.TrackingNumber.ToString("D"));

            return StartActivity(context, activity);
        }

        public StartedActivity? StartCompensateActivity<TActivity, TLog>(ConsumeContext<RoutingSlip> context)
            where TActivity : ICompensateActivity<TLog>
            where TLog : class
        {
            var activity = new System.Diagnostics.Activity(_name);

            activity.AddBaggage(DiagnosticHeaders.TrackingNumber, context.Message.TrackingNumber.ToString("D"));

            activity.AddTag(DiagnosticHeaders.PeerService, "Compensate");
            activity.AddTag(DiagnosticHeaders.PeerAddress, TypeMetadataCache<TActivity>.DiagnosticAddress);
            activity.AddTag(DiagnosticHeaders.ActivityType, TypeMetadataCache<TActivity>.ShortName);
            activity.AddTag(DiagnosticHeaders.LogType, TypeMetadataCache<TLog>.ShortName);
            activity.AddTag(DiagnosticHeaders.TrackingNumber, context.Message.TrackingNumber.ToString("D"));

            return StartActivity(context, activity);
        }

        StartedActivity? StartActivity<T>(ConsumeContext<T> context, System.Diagnostics.Activity activity)
            where T : class
        {
            activity.AddTag(DiagnosticHeaders.ServiceKind, DiagnosticHeaders.Kind.Consumer);
            activity.AddTag(DiagnosticHeaders.PeerHost, HostMetadataCache.Host.MachineName);

            var startActivity = _source.StartActivity(activity, context);

            EnabledScope? scope = LogContext.BeginScope();

            return new StartedActivity(_source, startActivity, scope);
        }

        System.Diagnostics.Activity CreateActivity(Headers headers)
        {
            var activity = new System.Diagnostics.Activity(_name);

            var parentActivityId = GetParentActivityId(headers);
            if (parentActivityId != null)
                activity.SetParentId(parentActivityId);

            return activity;
        }

        static string GetParentActivityId(Headers headers)
        {
            string parentActivityId = null;
            if (headers.TryGetHeader(DiagnosticHeaders.ActivityId, out var headerValue)
                && headerValue is string activityId && !string.IsNullOrWhiteSpace(activityId))
            {
                parentActivityId = activityId;
            }

            return parentActivityId;
        }
    }
}
