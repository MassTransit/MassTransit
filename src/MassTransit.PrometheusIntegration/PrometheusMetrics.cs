namespace MassTransit
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Courier;
    using Prometheus;


    public static class PrometheusMetrics
    {
        static readonly ConcurrentDictionary<string, string> _labelCache = new ConcurrentDictionary<string, string>();

        static bool _isConfigured;
        static string _serviceLabel;
        static Gauge _busInstances;
        static Gauge _endpointInstances;
        static Counter _receiveTotal;
        static Counter _receiveFaultTotal;
        static Gauge _receiveInProgress;
        static Histogram _receiveDuration;
        static Counter _consumeTotal;
        static Counter _consumeFaultTotal;
        static Counter _consumeRetryTotal;
        static Counter _publishTotal;
        static Counter _publishFaultTotal;
        static Counter _sendTotal;
        static Counter _sendFaultTotal;
        static Counter _executeTotal;
        static Counter _executeFaultTotal;
        static Counter _compensateTotal;
        static Counter _compensateFailureTotal;
        static Gauge _consumerInProgress;
        static Gauge _handlerInProgress;
        static Gauge _sagaInProgress;
        static Gauge _executeInProgress;
        static Gauge _compensateInProgress;
        static Histogram _consumeDuration;
        static Histogram _deliveryDuration;
        static Histogram _executeDuration;
        static Histogram _compensateDuration;

        static readonly char[] _delimiters = {'<', '>'};

        public static void BusStarted()
        {
            _busInstances.WithLabels(_serviceLabel).Inc();
        }

        public static void BusStopped()
        {
            _busInstances.WithLabels(_serviceLabel).Dec();
        }

        public static void EndpointReady(ReceiveEndpointReady ready)
        {
            var endpointLabel = GetEndpointLabel(ready.InputAddress);

            _endpointInstances.WithLabels(_serviceLabel, endpointLabel).Inc();
        }

        public static void EndpointCompleted(ReceiveEndpointCompleted completed)
        {
            var endpointLabel = GetEndpointLabel(completed.InputAddress);

            _endpointInstances.WithLabels(_serviceLabel, endpointLabel).Dec();
        }

        public static void MeasureReceived(ReceiveContext context, Exception exception = default)
        {
            var endpointLabel = GetEndpointLabel(context.InputAddress);

            _receiveTotal.Labels(_serviceLabel, endpointLabel).Inc();
            _receiveDuration.Labels(_serviceLabel, endpointLabel).Observe(context.ElapsedTime.TotalSeconds);

            if (exception != null)
            {
                var exceptionType = exception.GetType().Name;
                _receiveFaultTotal.Labels(_serviceLabel, endpointLabel, exceptionType).Inc();
            }
        }

        public static void MeasureConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception = default)
            where T : class
        {
            var messageType = GetMessageTypeLabel<T>();
            var cleanConsumerType = GetConsumerTypeLabel(consumerType, TypeCache<T>.ShortName, messageType);

            _consumeTotal.Labels(_serviceLabel, messageType, cleanConsumerType).Inc();
            _consumeDuration.Labels(_serviceLabel, messageType, cleanConsumerType).Observe(duration.TotalSeconds);

            if (exception != null)
            {
                var exceptionType = exception.GetType().Name;
                _consumeFaultTotal.Labels(_serviceLabel, messageType, cleanConsumerType, exceptionType).Inc();
            }

            var retryAttempt = context.GetRetryAttempt();
            if (retryAttempt > 0)
                _consumeRetryTotal.Inc(retryAttempt);

            if (!context.SentTime.HasValue)
                return;

            var deliveryDuration = DateTime.UtcNow - context.SentTime.Value;
            if (deliveryDuration < TimeSpan.Zero)
                deliveryDuration = TimeSpan.Zero;

            _deliveryDuration.Labels(_serviceLabel, messageType, cleanConsumerType).Observe(deliveryDuration.TotalSeconds);
        }

        public static void MeasureExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context, Exception exception = default)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var argumentType = GetArgumentTypeLabel<TArguments>();

            _executeTotal.Labels(_serviceLabel, context.ActivityName, argumentType).Inc();
            _executeDuration.Labels(_serviceLabel, context.ActivityName, argumentType).Observe(context.Elapsed.TotalSeconds);

            if (exception != null)
            {
                var exceptionType = exception.GetType().Name;
                _executeFaultTotal.Labels(_serviceLabel, context.ActivityName, argumentType, exceptionType).Inc();
            }
        }

        public static void MeasureCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context, Exception exception = default)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var logType = GetLogTypeLabel<TLog>();

            _compensateTotal.Labels(_serviceLabel, context.ActivityName, logType).Inc();
            _compensateDuration.Labels(_serviceLabel, context.ActivityName, logType).Observe(context.Elapsed.TotalSeconds);

            if (exception != null)
            {
                var exceptionType = exception.GetType().Name;
                _compensateFailureTotal.Labels(_serviceLabel, context.ActivityName, logType, exceptionType).Inc();
            }
        }

        public static void MeasurePublish<T>(Exception exception = default)
            where T : class
        {
            var messageType = GetMessageTypeLabel<T>();

            _publishTotal.Labels(_serviceLabel, messageType).Inc();

            if (exception != null)
            {
                var exceptionType = exception.GetType().Name;
                _publishFaultTotal.Labels(_serviceLabel, messageType, exceptionType).Inc();
            }
        }

        public static void MeasureSend<T>(Exception exception = default)
            where T : class
        {
            var messageType = GetMessageTypeLabel<T>();

            _sendTotal.Labels(_serviceLabel, messageType).Inc();

            if (exception != null)
            {
                var exceptionType = exception.GetType().Name;
                _sendFaultTotal.Labels(_serviceLabel, messageType, exceptionType).Inc();
            }
        }

        public static IDisposable TrackReceiveInProgress(ReceiveContext context)
        {
            var endpointLabel = GetEndpointLabel(context.InputAddress);

            return _receiveInProgress.Labels(_serviceLabel, endpointLabel).TrackInProgress();
        }

        public static IDisposable TrackConsumerInProgress<TConsumer, TMessage>()
            where TConsumer : class
            where TMessage : class
        {
            var messageType = GetMessageTypeLabel<TMessage>();
            var cleanConsumerType = GetConsumerTypeLabel(TypeCache<TConsumer>.ShortName, TypeCache<TMessage>.ShortName, messageType);

            return _consumerInProgress.Labels(_serviceLabel, messageType, cleanConsumerType).TrackInProgress();
        }

        public static IDisposable TrackSagaInProgress<TSaga, TMessage>()
            where TSaga : class, ISaga
            where TMessage : class
        {
            var messageType = GetMessageTypeLabel<TMessage>();
            var cleanConsumerType = GetConsumerTypeLabel(TypeCache<TSaga>.ShortName, TypeCache<TMessage>.ShortName, messageType);

            return _sagaInProgress.Labels(_serviceLabel, messageType, cleanConsumerType).TrackInProgress();
        }

        public static IDisposable TrackExecuteActivityInProgress<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var argumentType = GetArgumentTypeLabel<TArguments>();

            return _executeInProgress.Labels(_serviceLabel, context.ActivityName, argumentType).TrackInProgress();
        }

        public static IDisposable TrackCompensateActivityInProgress<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var argumentType = GetArgumentTypeLabel<TLog>();

            return _compensateInProgress.Labels(_serviceLabel, context.ActivityName, argumentType).TrackInProgress();
        }

        public static IDisposable TrackHandlerInProgress<TMessage>()
            where TMessage : class
        {
            var messageType = GetMessageTypeLabel<TMessage>();

            return _handlerInProgress.Labels(_serviceLabel, messageType).TrackInProgress();
        }

        public static void TryConfigure(string serviceName, PrometheusMetricsOptions options)
        {
            if (_isConfigured)
                return;

            _serviceLabel = serviceName;

            string[] serviceLabels = {options.ServiceNameLabel};

            string[] endpointLabels = {options.ServiceNameLabel, options.EndpointLabel};
            string[] endpointFaultLabels = {options.ServiceNameLabel, options.EndpointLabel, options.ExceptionTypeLabel};

            string[] messageLabels = {options.ServiceNameLabel, options.MessageTypeLabel};
            string[] messageFaultLabels = {options.ServiceNameLabel, options.MessageTypeLabel, options.ExceptionTypeLabel};

            string[] executeLabels = {options.ServiceNameLabel, options.ActivityNameLabel, options.ArgumentTypeLabel};
            string[] executeFaultLabels = {options.ServiceNameLabel, options.ActivityNameLabel, options.ArgumentTypeLabel, options.ExceptionTypeLabel};

            string[] compensateLabels = {options.ServiceNameLabel, options.ActivityNameLabel, options.LogTypeLabel};
            string[] compensateFailureLabels = {options.ServiceNameLabel, options.ActivityNameLabel, options.LogTypeLabel, options.ExceptionTypeLabel};

            string[] consumerLabels = {options.ServiceNameLabel, options.MessageTypeLabel, options.ConsumerTypeLabel};
            string[] consumerFaultLabels = {options.ServiceNameLabel, options.MessageTypeLabel, options.ConsumerTypeLabel, options.ExceptionTypeLabel};

            // Counters

            _receiveTotal = Metrics.CreateCounter(
                options.ReceiveTotal,
                "Total number of messages received",
                new CounterConfiguration {LabelNames = endpointLabels});

            _receiveFaultTotal = Metrics.CreateCounter(
                options.ReceiveFaultTotal,
                "Total number of messages receive faults",
                new CounterConfiguration {LabelNames = endpointFaultLabels});

            _consumeTotal = Metrics.CreateCounter(
                options.ConsumeTotal,
                "Total number of messages consumed",
                new CounterConfiguration {LabelNames = consumerLabels});

            _consumeFaultTotal = Metrics.CreateCounter(
                options.ConsumeFaultTotal,
                "Total number of message consume faults",
                new CounterConfiguration {LabelNames = consumerFaultLabels});

            _consumeRetryTotal = Metrics.CreateCounter(
                options.ConsumeRetryTotal,
                "Total number of message consume faults",
                new CounterConfiguration {LabelNames = consumerFaultLabels});

            _publishTotal = Metrics.CreateCounter(
                options.PublishTotal,
                "Total number of messages published",
                new CounterConfiguration {LabelNames = messageLabels});

            _publishFaultTotal = Metrics.CreateCounter(
                options.PublishFaultTotal,
                "Total number of message publish faults",
                new CounterConfiguration {LabelNames = messageFaultLabels});

            _sendTotal = Metrics.CreateCounter(
                options.SendTotal,
                "Total number of messages sent",
                new CounterConfiguration {LabelNames = messageLabels});

            _sendFaultTotal = Metrics.CreateCounter(
                options.SendFaultTotal,
                "Total number of message send faults",
                new CounterConfiguration {LabelNames = messageFaultLabels});

            _executeTotal = Metrics.CreateCounter(
                options.ActivityExecuteTotal,
                "Total number of activities executed",
                new CounterConfiguration {LabelNames = executeLabels});

            _executeFaultTotal = Metrics.CreateCounter(
                options.ActivityExecuteFaultTotal,
                "Total number of activity execution faults",
                new CounterConfiguration {LabelNames = executeFaultLabels});

            _compensateTotal = Metrics.CreateCounter(
                options.ActivityCompensateTotal,
                "Total number of activities compensated",
                new CounterConfiguration {LabelNames = compensateLabels});

            _compensateFailureTotal = Metrics.CreateCounter(
                options.ActivityCompensateFailureTotal,
                "Total number of activity compensation failures",
                new CounterConfiguration {LabelNames = compensateFailureLabels});

            // Gauges

            _busInstances = Metrics.CreateGauge(
                options.BusInstances,
                "Number of bus instances",
                new GaugeConfiguration {LabelNames = serviceLabels});

            _endpointInstances = Metrics.CreateGauge(
                options.EndpointInstances,
                "Number of receive endpoint instances",
                new GaugeConfiguration {LabelNames = endpointLabels});

            _receiveInProgress = Metrics.CreateGauge(
                options.ReceiveInProgress,
                "Number of messages being received",
                new GaugeConfiguration {LabelNames = endpointLabels});

            _handlerInProgress = Metrics.CreateGauge(
                options.HandlerInProgress,
                "Number of handlers in progress",
                new GaugeConfiguration {LabelNames = messageLabels});

            _consumerInProgress = Metrics.CreateGauge(
                options.ConsumerInProgress,
                "Number of consumers in progress",
                new GaugeConfiguration {LabelNames = consumerLabels});

            _sagaInProgress = Metrics.CreateGauge(
                options.SagaInProgress,
                "Number of sagas in progress",
                new GaugeConfiguration {LabelNames = consumerLabels});

            _executeInProgress = Metrics.CreateGauge(
                options.ExecuteInProgress,
                "Number of activity executions in progress",
                new GaugeConfiguration {LabelNames = executeLabels});

            _compensateInProgress = Metrics.CreateGauge(
                options.CompensateInProgress,
                "Number of activity compensations in progress",
                new GaugeConfiguration {LabelNames = compensateLabels});

            // Histograms

            _receiveDuration = Metrics.CreateHistogram(
                options.ReceiveDuration,
                "Elapsed time spent receiving a message, in seconds",
                new HistogramConfiguration
                {
                    LabelNames = endpointLabels,
                    Buckets = options.HistogramBuckets
                });

            _consumeDuration = Metrics.CreateHistogram(
                options.ConsumeDuration,
                "Elapsed time spent consuming a message, in seconds",
                new HistogramConfiguration
                {
                    LabelNames = consumerLabels,
                    Buckets = options.HistogramBuckets
                });

            _deliveryDuration = Metrics.CreateHistogram(
                options.DeliveryDuration,
                "Elapsed time between when the message was sent and when it was consumed, in seconds.",
                new HistogramConfiguration
                {
                    LabelNames = consumerLabels,
                    Buckets = options.HistogramBuckets
                });

            _executeDuration = Metrics.CreateHistogram(
                options.ActivityExecuteDuration,
                "Elapsed time spent executing an activity, in seconds",
                new HistogramConfiguration
                {
                    LabelNames = executeLabels,
                    Buckets = options.HistogramBuckets
                });

            _compensateDuration = Metrics.CreateHistogram(
                options.ActivityCompensateDuration,
                "Elapsed time spent compensating an activity, in seconds",
                new HistogramConfiguration
                {
                    LabelNames = compensateLabels,
                    Buckets = options.HistogramBuckets
                });

            _isConfigured = true;
        }

        static string GetConsumerTypeLabel(string consumerType, string messageType, string messageLabel)
        {
            return _labelCache.GetOrAdd(consumerType, type =>
            {
                if (type.StartsWith("MassTransit.MessageHandler<"))
                    return "Handler";

                var genericMessageType = "<" + messageType + ">";
                if (type.IndexOf(genericMessageType, StringComparison.Ordinal) >= 0)
                    type = type.Replace(genericMessageType, "_" + messageLabel);

                return CleanupLabel(type);
            });
        }

        static string CleanupLabel(string label)
        {
            string SimpleClean(string text)
            {
                return text.Split('.', '+').Last();
            }

            var indexOf = label.IndexOfAny(_delimiters);
            if (indexOf >= 0)
            {
                if (label[indexOf] == '<')
                    return SimpleClean(label.Substring(0, indexOf)) + "_" + CleanupLabel(label.Substring(indexOf + 1));

                if (label[indexOf] == '>')
                    return SimpleClean(label.Substring(0, indexOf)) + CleanupLabel(label.Substring(indexOf + 1));

                return SimpleClean(label);
            }

            return SimpleClean(label);
        }

        static string GetArgumentTypeLabel<TArguments>()
        {
            return _labelCache.GetOrAdd(TypeCache<TArguments>.ShortName, type => FormatTypeName(new StringBuilder(), typeof(TArguments))
                .Replace("Arguments", ""));
        }

        static string GetLogTypeLabel<TLog>()
        {
            return _labelCache.GetOrAdd(TypeCache<TLog>.ShortName, type => FormatTypeName(new StringBuilder(), typeof(TLog)).Replace("Log", ""));
        }

        static string GetEndpointLabel(Uri inputAddress)
        {
            return inputAddress?.AbsolutePath.Split('/').LastOrDefault()?.Replace(".", "_").Replace("/", "_");
        }

        static string GetMessageTypeLabel<TMessage>()
        {
            return _labelCache.GetOrAdd(TypeCache<TMessage>.ShortName, type => FormatTypeName(new StringBuilder(), typeof(TMessage)));
        }

        static string FormatTypeName(StringBuilder sb, Type type)
        {
            if (type.IsGenericParameter)
                return "";

            if (type.GetTypeInfo().IsGenericType)
            {
                var name = type.GetGenericTypeDefinition().Name;

                //remove `1
                var index = name.IndexOf('`');
                if (index > 0)
                    name = name.Remove(index);

                sb.Append(name);
                sb.Append('_');
                Type[] arguments = type.GetTypeInfo().GenericTypeArguments;
                for (var i = 0; i < arguments.Length; i++)
                {
                    if (i > 0)
                        sb.Append('_');

                    FormatTypeName(sb, arguments[i]);
                }
            }
            else
                sb.Append(type.Name);

            return sb.ToString();
        }
    }
}
