#nullable enable
namespace MassTransit.Logging
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Courier.Contracts;
    using Microsoft.Extensions.Logging;
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
            var parentActivityContext = System.Diagnostics.Activity.Current?.Context ?? default;

            var activity = _source.CreateActivity(transportContext.ActivityName, ActivityKind.Producer, parentActivityContext);
            if (activity == null)
                return null;

            activity.AddTag(DiagnosticHeaders.Messaging.System, transportContext.ActivitySystem);
            activity.AddTag(DiagnosticHeaders.Messaging.Destination, transportContext.ActivityDestination);
            activity.AddTag(DiagnosticHeaders.Messaging.Operation, "send");

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

            return new StartedActivity(activity);
        }

        public StartedActivity? StartReceiveActivity(string name, string inputAddress, string endpointName, ReceiveContext context,
            params (string Key, string Value)[] tags)
        {
            var parentActivityContext = GetParentActivityContext(context.TransportHeaders);

            var activity = _source.CreateActivity(name, ActivityKind.Consumer, parentActivityContext);
            if (activity == null)
                return null;

            activity.Start();

            if (activity.IsAllDataRequested)
            {
                activity.AddTag(DiagnosticHeaders.Messaging.Destination, endpointName);
                activity.AddTag(DiagnosticHeaders.Messaging.Operation, "receive");
                activity.AddTag(DiagnosticHeaders.InputAddress, inputAddress);

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

            return new StartedActivity(activity);
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

        public StartedActivity? StartSagaStateMachineActivity<TSaga, T>(BehaviorContext<TSaga, T> context)
            where TSaga : class, ISaga
            where T : class
        {
            return StartActivity(activity =>
            {
                activity.AddTag(DiagnosticHeaders.SagaId, context.Saga.CorrelationId.ToString("D"));
                activity.AddTag(DiagnosticHeaders.ServiceName, context.StateMachine.Name);
                activity.AddTag(DiagnosticHeaders.PeerAddress, MessageTypeCache<T>.DiagnosticAddress);
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

            var activity = _source.CreateActivity(operationName, ActivityKind.Consumer, currentActivity.Context);
            if (activity == null)
                return null;

            activity.Start();

            activity.AddTag(DiagnosticHeaders.Messaging.Operation, "process");

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
