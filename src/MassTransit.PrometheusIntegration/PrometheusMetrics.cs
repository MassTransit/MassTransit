namespace MassTransit.PrometheusIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Courier;
    using Metadata;
    using Prometheus;
    using Saga;


    public static class PrometheusMetrics
    {
        static readonly ConcurrentDictionary<string, string> _consumerTypeCache = new ConcurrentDictionary<string, string>();

        static bool _isConfigured;
        static string _serviceLabel;
        static Gauge _busInstances;
        static Counter _receiveTotal;
        static Counter _receiveFaultTotal;
        static Gauge _receiveInProgress;
        static Histogram _receiveDuration;
        static Counter _messageConsumeTotal;
        static Counter _messageConsumeFaultTotal;
        static Counter _messagePublishTotal;
        static Counter _messagePublishFaultTotal;
        static Counter _messageSendTotal;
        static Counter _messageSendFaultTotal;
        static Gauge _consumerInProgress;
        static Gauge _handlerInProgress;
        static Gauge _sagaInProgress;
        static Gauge _activityInProgress;
        static Histogram _messageConsumeDuration;
        static Histogram _messageDeliveryDuration;

        public static void BusStarted()
        {
            _busInstances.WithLabels(_serviceLabel).Inc();
        }

        public static void BusStopped()
        {
            _busInstances.WithLabels(_serviceLabel).Dec();
        }

        public static void MeasureReceived(ReceiveContext context, Exception exception = default)
        {
            var endpointLabel = GetEndpointLabel(context);

            _receiveTotal.Labels(_serviceLabel, endpointLabel).Inc();
            _receiveDuration.Labels(_serviceLabel, endpointLabel).Observe(context.ElapsedTime.TotalSeconds);

            if (exception != null)
            {
                var exceptionType = exception.GetType().Name;
                _receiveFaultTotal.Labels(_serviceLabel, endpointLabel, exceptionType).Inc();
            }
        }

        public static void MeasureConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception = default)
            where T : class
        {
            var messageType = GetMessageTypeLabel<T>();
            var cleanConsumerType = GetConsumerTypeLabel(consumerType);

            _messageConsumeTotal.Labels(_serviceLabel, messageType, cleanConsumerType).Inc();
            _messageConsumeDuration.Labels(_serviceLabel, messageType, cleanConsumerType).Observe(duration.TotalSeconds);

            if (exception != null)
            {
                var exceptionType = exception.GetType().Name;
                _messageConsumeFaultTotal.Labels(_serviceLabel, messageType, cleanConsumerType, exceptionType).Inc();
            }

            if (!context.SentTime.HasValue)
                return;

            var deliveryDuration = DateTime.UtcNow - context.SentTime.Value;
            if (deliveryDuration < TimeSpan.Zero)
                deliveryDuration = TimeSpan.Zero;

            _messageDeliveryDuration.Labels(_serviceLabel, messageType, cleanConsumerType).Observe(deliveryDuration.TotalSeconds);
        }

        public static void MeasurePublished<T>(Exception exception = default)
            where T : class
        {
            var messageType = GetMessageTypeLabel<T>();

            _messagePublishTotal.Labels(_serviceLabel, messageType).Inc();

            if (exception != null)
            {
                var exceptionType = exception.GetType().Name;
                _messagePublishFaultTotal.Labels(_serviceLabel, messageType, exceptionType).Inc();
            }
        }

        public static void MeasureSent<T>(Exception exception = default)
            where T : class
        {
            var messageType = GetMessageTypeLabel<T>();

            _messageSendTotal.Labels(_serviceLabel, messageType).Inc();

            if (exception != null)
            {
                var exceptionType = exception.GetType().Name;
                _messageSendFaultTotal.Labels(_serviceLabel, messageType, exceptionType).Inc();
            }
        }

        public static IDisposable TrackReceiveInProgress(ReceiveContext context)
        {
            var endpointLabel = GetEndpointLabel(context);

            return _receiveInProgress.Labels(_serviceLabel, endpointLabel).TrackInProgress();
        }

        public static IDisposable TrackConsumerInProgress<TConsumer, TMessage>()
            where TConsumer : class
            where TMessage : class
        {
            var messageType = GetMessageTypeLabel<TMessage>();
            var cleanConsumerType = GetConsumerTypeLabel(TypeMetadataCache<TConsumer>.ShortName);

            return _consumerInProgress.Labels(_serviceLabel, messageType, cleanConsumerType).TrackInProgress();
        }

        public static IDisposable TrackSagaInProgress<TSaga, TMessage>()
            where TSaga : class, ISaga
            where TMessage : class
        {
            var messageType = GetMessageTypeLabel<TMessage>();
            var cleanConsumerType = GetConsumerTypeLabel(TypeMetadataCache<TSaga>.ShortName);

            return _sagaInProgress.Labels(_serviceLabel, messageType, cleanConsumerType).TrackInProgress();
        }

        public static IDisposable TrackExecuteActivityInProgress<TActivity, TArguments>()
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var messageType = GetMessageTypeLabel<TArguments>();
            var cleanConsumerType = GetConsumerTypeLabel(TypeMetadataCache<TActivity>.ShortName);

            return _activityInProgress.Labels(_serviceLabel, messageType, cleanConsumerType).TrackInProgress();
        }

        public static IDisposable TrackCompensateActivityInProgress<TActivity, TLog>()
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var messageType = GetMessageTypeLabel<TLog>();
            var cleanConsumerType = GetConsumerTypeLabel(TypeMetadataCache<TActivity>.ShortName);

            return _activityInProgress.Labels(_serviceLabel, messageType, cleanConsumerType).TrackInProgress();
        }

        public static IDisposable TrackHandlerInProgress<TMessage>()
            where TMessage : class
        {
            var messageType = GetMessageTypeLabel<TMessage>();

            return _handlerInProgress.Labels(_serviceLabel, messageType).TrackInProgress();
        }

        static string GetMessageTypeLabel<TMessage>()
            where TMessage : class
        {
            var messageType = typeof(TMessage).Name;
            return messageType;
        }

        static string GetConsumerTypeLabel(string consumerType)
        {
            return _consumerTypeCache.GetOrAdd(consumerType, type => type.Split('.', '+').Last().Replace("<", "_").Replace(">", "_"));
        }

        static string GetEndpointLabel(ReceiveContext context)
        {
            return context.InputAddress?.AbsolutePath.Split('/').LastOrDefault();
        }

        public static void TryConfigure(string serviceName, PrometheusMetricsOptions options)
        {
            if (_isConfigured)
                return;

            _serviceLabel = serviceName;

            string[] serviceLabels = {options.ServiceNameLabel};

            string[] receiveLabels = {options.ServiceNameLabel, options.EndpointLabel};
            string[] receiveFaultLabels = {options.ServiceNameLabel, options.EndpointLabel, options.ExceptionTypeLabel};

            string[] messageLabels = {options.ServiceNameLabel, options.MessageTypeLabel,};
            string[] messageFaultLabels = {options.ServiceNameLabel, options.MessageTypeLabel, options.ExceptionTypeLabel};

            string[] consumerLabels = {options.ServiceNameLabel, options.MessageTypeLabel, options.ConsumerTypeLabel};
            string[] consumerFaultLabels = {options.ServiceNameLabel, options.MessageTypeLabel, options.ConsumerTypeLabel, options.ExceptionTypeLabel};

            // Counters

            _receiveTotal = Metrics.CreateCounter(
                options.ReceiveTotal,
                "Total number of messages received",
                new CounterConfiguration {LabelNames = receiveLabels});

            _receiveFaultTotal = Metrics.CreateCounter(
                options.ReceiveFaultTotal,
                "Total number of messages receive faults",
                new CounterConfiguration {LabelNames = receiveFaultLabels});

            _messageConsumeTotal = Metrics.CreateCounter(
                options.MessageConsumeTotal,
                "Total number of messages consumed",
                new CounterConfiguration {LabelNames = consumerLabels});

            _messageConsumeFaultTotal = Metrics.CreateCounter(
                options.MessageConsumeFaultTotal,
                "Total number of message consume faults",
                new CounterConfiguration {LabelNames = consumerFaultLabels});

            _messagePublishTotal = Metrics.CreateCounter(
                options.MessagePublishTotal,
                "Total number of messages published",
                new CounterConfiguration {LabelNames = messageLabels});

            _messagePublishFaultTotal = Metrics.CreateCounter(
                options.MessagePublishFaultTotal,
                "Total number of message publish faults",
                new CounterConfiguration {LabelNames = messageFaultLabels});

            _messageSendTotal = Metrics.CreateCounter(
                options.MessageSendTotal,
                "Total number of messages sent",
                new CounterConfiguration {LabelNames = messageLabels});

            _messageSendFaultTotal = Metrics.CreateCounter(
                options.MessageSendFaultTotal,
                "Total number of message send fault",
                new CounterConfiguration {LabelNames = messageFaultLabels});

            // Gauges

            _busInstances = Metrics.CreateGauge(
                options.BusInstances,
                "Number of bus instances",
                new GaugeConfiguration {LabelNames = serviceLabels});

            _receiveInProgress = Metrics.CreateGauge(
                options.ReceiveInProgress,
                "Number of messages being received",
                new GaugeConfiguration {LabelNames = receiveLabels});

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

            _activityInProgress = Metrics.CreateGauge(
                options.ActivityInProgress,
                "Number of activities in progress",
                new GaugeConfiguration {LabelNames = consumerLabels});

            // Histograms

            _receiveDuration = Metrics.CreateHistogram(
                options.ReceiveDuration,
                "Elapsed time spent receiving messages, in seconds",
                new HistogramConfiguration
                {
                    LabelNames = receiveLabels,
                    Buckets = options.HistogramBuckets
                });

            _messageConsumeDuration = Metrics.CreateHistogram(
                options.MessageConsumeDuration,
                "Elapsed time spent consuming a message, in seconds",
                new HistogramConfiguration
                {
                    LabelNames = consumerLabels,
                    Buckets = options.HistogramBuckets
                });

            _messageDeliveryDuration = Metrics.CreateHistogram(
                options.MessageDeliveryDuration,
                "Elapsed time between when the message was sent and when it was consumed, in seconds.",
                new HistogramConfiguration
                {
                    LabelNames = consumerLabels,
                    Buckets = options.HistogramBuckets
                });

            _isConfigured = true;
        }
    }
}
