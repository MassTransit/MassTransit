#nullable enable
namespace MassTransit.Logging
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Courier.Contracts;
    using Microsoft.Extensions.Logging;
    using Middleware;
    using Transports;


    public class BusLogContext :
        ILogContext
    {
        readonly ILoggerFactory _loggerFactory;
        readonly ILogContext _messageLogger;
        readonly ActivitySource _source;

        public BusLogContext(ILoggerFactory loggerFactory, ActivitySource source)
        {
            _source = source;
            _loggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger(LogCategoryName.MassTransit);


            _messageLogger = new BusLogContext(source, loggerFactory, loggerFactory.CreateLogger("MassTransit.Messages"));
        }

        BusLogContext(ActivitySource source, ILoggerFactory loggerFactory, ILogContext messageLogger, ILogger logger)
        {
            _source = source;
            _loggerFactory = loggerFactory;
            _messageLogger = messageLogger;
            Logger = logger;
        }

        BusLogContext(ActivitySource source, ILoggerFactory loggerFactory, ILogger logger)
        {
            _source = source;
            _loggerFactory = loggerFactory;
            Logger = logger;

            _messageLogger = this;
        }

        ILogContext ILogContext.Messages => _messageLogger;

        public ILogContext CreateLogContext(string categoryName)
        {
            var logger = _loggerFactory.CreateLogger(categoryName);

            return new BusLogContext(_source, _loggerFactory, _messageLogger, logger);
        }

        public ILogger Logger { get; }

        public EnabledLogger? Critical => Logger.IsEnabled(LogLevel.Critical) ? new EnabledLogger(Logger, LogLevel.Critical) : default(EnabledLogger?);

        public EnabledLogger? Debug => Logger.IsEnabled(LogLevel.Debug) ? new EnabledLogger(Logger, LogLevel.Debug) : default(EnabledLogger?);

        public EnabledLogger? Error => Logger.IsEnabled(LogLevel.Error) ? new EnabledLogger(Logger, LogLevel.Error) : default(EnabledLogger?);

        public EnabledLogger? Info => Logger.IsEnabled(LogLevel.Information) ? new EnabledLogger(Logger, LogLevel.Information) : default(EnabledLogger?);

        public EnabledLogger? Trace => Logger.IsEnabled(LogLevel.Trace) ? new EnabledLogger(Logger, LogLevel.Trace) : default(EnabledLogger?);

        public EnabledLogger? Warning => Logger.IsEnabled(LogLevel.Warning) ? new EnabledLogger(Logger, LogLevel.Warning) : default(EnabledLogger?);

        public StartedActivity? StartSendActivity<T>(SendTransportContext transportContext, SendContext<T> context, params (string Key, object Value)[] tags)
            where T : class
        {
            var activity = _source.CreateActivity(transportContext.ActivityName, ActivityKind.Producer);
            if (activity == null)
                return null;

            activity.SetTag(DiagnosticHeaders.Messaging.System, transportContext.ActivitySystem);
            activity.SetTag(DiagnosticHeaders.Messaging.Destination, transportContext.ActivityDestination);
            activity.SetTag(DiagnosticHeaders.Messaging.Operation, "send");

            return PopulateSendActivity<T>(context, activity, tags);
        }

        public StartedActivity? StartOutboxSendActivity<T>(SendContext<T> context)
            where T : class
        {
            var parentActivityContext = System.Diagnostics.Activity.Current?.Context ?? default;

            var activity = _source.CreateActivity("outbox send", ActivityKind.Producer, parentActivityContext);
            if (activity == null)
                return null;

            activity.SetTag(DiagnosticHeaders.Messaging.Operation, "send");

            return PopulateSendActivity<T>(context, activity);
        }

        public StartedActivity? StartOutboxDeliverActivity(OutboxMessageContext context)
        {
            var parentActivityContext = GetParentActivityContext(context.Headers);

            var activity = _source.CreateActivity("outbox process", ActivityKind.Client, parentActivityContext);
            if (activity == null)
                return null;

            activity.Start();

            return new StartedActivity(activity);
        }

        public StartedActivity? StartReceiveActivity(string name, string inputAddress, string endpointName, ReceiveContext context)
        {
            var parentActivityContext = GetParentActivityContext(context.TransportHeaders);

            var activity = _source.CreateActivity(name, ActivityKind.Consumer, parentActivityContext);
            if (activity == null)
                return null;

            activity.Start();

            if (activity.IsAllDataRequested)
            {
                activity.SetTag(DiagnosticHeaders.Messaging.Destination, endpointName);
                activity.SetTag(DiagnosticHeaders.Messaging.Operation, "receive");
                activity.SetTag(DiagnosticHeaders.InputAddress, inputAddress);

                if ((context.TransportHeaders.TryGetHeader(MessageHeaders.TransportMessageId, out var messageIdHeader)
                        || context.TransportHeaders.TryGetHeader(MessageHeaders.MessageId, out messageIdHeader))
                    && messageIdHeader is string text)
                    activity.SetTag(DiagnosticHeaders.Messaging.TransportMessageId, text);
            }

            return new StartedActivity(activity);
        }

        public StartedActivity? StartConsumerActivity<TConsumer, T>(ConsumeContext<T> context)
            where T : class
        {
            return StartActivity(activity =>
            {
                activity.SetTag(DiagnosticHeaders.ConsumerType, TypeCache<TConsumer>.ShortName);
                activity.SetTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<T>.DiagnosticAddress);
            });
        }

        public StartedActivity? StartHandlerActivity<T>(ConsumeContext<T> context)
            where T : class
        {
            return StartActivity(activity =>
            {
                activity.SetTag(DiagnosticHeaders.ConsumerType, "Handler");
                activity.SetTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<T>.DiagnosticAddress);
            });
        }

        public StartedActivity? StartSagaActivity<TSaga, T>(SagaConsumeContext<TSaga, T> context)
            where TSaga : class, ISaga
            where T : class
        {
            return StartActivity(activity =>
            {
                activity.SetTag(DiagnosticHeaders.SagaId, context.Saga.CorrelationId.ToString("D"));
                activity.SetTag(DiagnosticHeaders.ConsumerType, TypeCache<TSaga>.ShortName);
                activity.SetTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<T>.DiagnosticAddress);
            });
        }

        public StartedActivity? StartSagaStateMachineActivity<TSaga, T>(BehaviorContext<TSaga, T> context)
            where TSaga : class, ISaga
            where T : class
        {
            return StartActivity(activity =>
            {
                activity.SetTag(DiagnosticHeaders.SagaId, context.Saga.CorrelationId.ToString("D"));
                activity.SetTag(DiagnosticHeaders.ConsumerType, context.StateMachine.Name);
                activity.SetTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<T>.DiagnosticAddress);
            });
        }

        public StartedActivity? StartExecuteActivity<TActivity, TArguments>(ConsumeContext<RoutingSlip> context)
            where TActivity : IExecuteActivity<TArguments>
            where TArguments : class
        {
            return StartActivity(activity =>
            {
                activity.SetTag(DiagnosticHeaders.TrackingNumber, context.Message.TrackingNumber.ToString("D"));
                activity.SetTag(DiagnosticHeaders.ConsumerType, TypeCache<TActivity>.ShortName);
                activity.SetTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<TArguments>.DiagnosticAddress);
            });
        }

        public StartedActivity? StartCompensateActivity<TActivity, TLog>(ConsumeContext<RoutingSlip> context)
            where TActivity : ICompensateActivity<TLog>
            where TLog : class
        {
            return StartActivity(activity =>
            {
                activity.SetTag(DiagnosticHeaders.TrackingNumber, context.Message.TrackingNumber.ToString("D"));
                activity.SetTag(DiagnosticHeaders.ConsumerType, TypeCache<TActivity>.ShortName);
                activity.SetTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<TLog>.DiagnosticAddress);
            });
        }

        public StartedActivity? StartGenericActivity(string operationName)
        {
            var activity = _source.CreateActivity(operationName, ActivityKind.Client);
            if (activity == null)
                return null;

            activity.Start();

            return new StartedActivity(activity);
        }

        static StartedActivity? PopulateSendActivity<T>(SendContext context, System.Diagnostics.Activity activity, params (string Key, object Value)[] tags)
            where T : class
        {
            var conversationId = context.ConversationId?.ToString("D");

            if (context.CorrelationId.HasValue)
                activity.SetBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId.Value.ToString("D"));
            if (conversationId != null)
                activity.SetBaggage(DiagnosticHeaders.Messaging.ConversationId, conversationId);

            activity.Start();

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

                activity.SetTag(DiagnosticHeaders.MessageTypes, string.Join(",", MessageTypeCache<T>.MessageTypeNames));

                for (var i = 0; i < tags.Length; i++)
                {
                    if (tags[i].Value != null)
                        activity.SetTag(tags[i].Key, tags[i].Value?.ToString());
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

            return new StartedActivity(activity);
        }

        StartedActivity? StartActivity(Action<System.Diagnostics.Activity> started)
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

            var activity = _source.CreateActivity(operationName, ActivityKind.Consumer);
            if (activity == null)
                return null;

            activity.Start();

            activity.SetTag(DiagnosticHeaders.Messaging.Operation, "process");

            if (activity.IsAllDataRequested)
                started(activity);

            return new StartedActivity(activity);
        }

        static ActivityContext GetParentActivityContext(Headers headers)
        {
            return headers.TryGetHeader(DiagnosticHeaders.ActivityId, out var headerValue) && headerValue is string activityId
                && ActivityContext.TryParse(activityId, null, out var activityContext)
                    ? activityContext
                    : default;
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<string, string> OperationNames = new ConcurrentDictionary<string, string>();
        }
    }
}
