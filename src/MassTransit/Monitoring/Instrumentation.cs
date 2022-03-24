namespace MassTransit.Monitoring
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Diagnostics.Metrics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Metadata;


    public static class Instrumentation
    {
        static readonly ConcurrentDictionary<string, string> _labelCache = new ConcurrentDictionary<string, string>();

        static bool _isConfigured;
        static string _serviceName;
        static Counter<long> _receiveTotal;
        static Counter<long> _receiveFaultTotal;
        static Counter<long> _receiveInProgress;
        static Histogram<double> _receiveDuration;
        static Counter<long> _consumeTotal;
        static Counter<long> _consumeFaultTotal;
        static Counter<long> _consumeRetryTotal;
        static Counter<long> _publishTotal;
        static Counter<long> _publishFaultTotal;
        static Counter<long> _sendTotal;
        static Counter<long> _sendFaultTotal;
        static Counter<long> _executeTotal;
        static Counter<long> _executeFaultTotal;
        static Counter<long> _compensateTotal;
        static Counter<long> _compensateFailureTotal;
        static Counter<long> _consumerInProgress;
        static Counter<long> _handlerInProgress;
        static Counter<long> _sagaInProgress;
        static Counter<long> _executeInProgress;
        static Counter<long> _compensateInProgress;
        static Histogram<double> _consumeDuration;
        static Histogram<double> _deliveryDuration;
        static Histogram<double> _executeDuration;
        static Histogram<double> _compensateDuration;

        static readonly char[] _delimiters = { '<', '>' };

        static Meter _meter;
        static InstrumentationOptions _options;

        public static void MeasureReceived(ReceiveContext context, Exception exception = default)
        {
            if (!_receiveTotal.Enabled)
                return;

            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _serviceName },
                { _options.EndpointLabel, GetEndpointLabel(context.InputAddress) },
            };

            _receiveTotal.Add(1, tagList);
            _receiveDuration.Record(context.ElapsedTime.TotalMilliseconds, tagList);

            if (exception != null)
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);

                _receiveFaultTotal.Add(1, tagList);
            }
        }

        public static void MeasureConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception = default)
            where T : class
        {
            if (!_consumeTotal.Enabled)
                return;

            var messageTypeLabel = GetMessageTypeLabel<T>();
            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _serviceName },
                { _options.MessageTypeLabel, messageTypeLabel },
                { _options.ConsumerTypeLabel, GetConsumerTypeLabel(consumerType, TypeCache<T>.ShortName, messageTypeLabel) }
            };

            _consumeTotal.Add(1, tagList);
            _consumeDuration.Record(duration.TotalMilliseconds, tagList);

            var retryAttempt = context.GetRetryAttempt();
            if (retryAttempt > 0)
                _consumeRetryTotal.Add(1, tagList);

            if (context.SentTime.HasValue)
            {
                var deliveryDuration = DateTime.UtcNow - context.SentTime.Value;
                if (deliveryDuration < TimeSpan.Zero)
                    deliveryDuration = TimeSpan.Zero;

                _deliveryDuration.Record(deliveryDuration.TotalMilliseconds, tagList);
            }

            if (exception != null)
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);

                _consumeFaultTotal.Add(1, tagList);
            }
        }

        public static void MeasureExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context, Exception exception = default)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            if (!_executeTotal.Enabled)
                return;

            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _serviceName },
                { _options.ActivityNameLabel, context.ActivityName },
                { _options.ArgumentTypeLabel, GetArgumentTypeLabel<TArguments>() }
            };

            _executeTotal.Add(1, tagList);
            _executeDuration.Record(context.Elapsed.TotalMilliseconds, tagList);

            if (exception != null)
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);

                _executeFaultTotal.Add(1, tagList);
            }
        }

        public static void MeasureCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context, Exception exception = default)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            if (!_compensateTotal.Enabled)
                return;

            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _serviceName },
                { _options.ActivityNameLabel, context.ActivityName },
                { _options.LogTypeLabel, GetLogTypeLabel<TLog>() }
            };

            _compensateTotal.Add(1, tagList);
            _compensateDuration.Record(context.Elapsed.TotalMilliseconds, tagList);

            if (exception != null)
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);

                _compensateFailureTotal.Add(1, tagList);
            }
        }

        public static void MeasurePublish<T>(Exception exception = default)
            where T : class
        {
            if (!_publishTotal.Enabled)
                return;
            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _serviceName },
                { _options.MessageTypeLabel, GetMessageTypeLabel<T>() },
            };

            _publishTotal.Add(1, tagList);

            if (exception != null)
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);
                _publishFaultTotal.Add(1, tagList);
            }
        }

        public static void MeasureSend<T>(Exception exception = default)
            where T : class
        {
            if (!_sendTotal.Enabled)
                return;

            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _serviceName },
                { _options.MessageTypeLabel, GetMessageTypeLabel<T>() },
            };

            _sendTotal.Add(1, tagList);

            if (exception != null)
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);
                _sendFaultTotal.Add(1, tagList);
            }
        }

        public static IDisposable TrackReceiveInProgress(ReceiveContext context)
        {
            if (!_receiveTotal.Enabled)
                return null;

            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _serviceName },
                { _options.EndpointLabel, GetEndpointLabel(context.InputAddress) },
            };

            return TrackInProgress(_receiveInProgress, tagList);
        }

        public static IDisposable TrackConsumerInProgress<TConsumer, TMessage>()
            where TConsumer : class
            where TMessage : class
        {
            if (!_consumeTotal.Enabled)
                return null;

            var messageTypeLabel = GetMessageTypeLabel<TMessage>();
            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _serviceName },
                { _options.MessageTypeLabel, messageTypeLabel },
                { _options.ConsumerTypeLabel, GetConsumerTypeLabel(TypeCache<TConsumer>.ShortName, TypeCache<TMessage>.ShortName, messageTypeLabel) }
            };

            return TrackInProgress(_consumerInProgress, tagList);
        }

        public static IDisposable TrackSagaInProgress<TSaga, TMessage>()
            where TSaga : class, ISaga
            where TMessage : class
        {
            if (!_sagaInProgress.Enabled)
                return null;

            var messageTypeLabel = GetMessageTypeLabel<TMessage>();
            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _serviceName },
                { _options.MessageTypeLabel, messageTypeLabel },
                { _options.ConsumerTypeLabel, GetConsumerTypeLabel(TypeCache<TSaga>.ShortName, TypeCache<TMessage>.ShortName, messageTypeLabel) }
            };

            return TrackInProgress(_sagaInProgress, tagList);
        }

        public static IDisposable TrackExecuteActivityInProgress<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            if (!_executeTotal.Enabled)
                return null;

            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _serviceName },
                { _options.ActivityNameLabel, context.ActivityName },
                { _options.ArgumentTypeLabel, GetArgumentTypeLabel<TArguments>() }
            };

            return TrackInProgress(_executeInProgress, tagList);
        }

        public static IDisposable TrackCompensateActivityInProgress<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            if (!_compensateTotal.Enabled)
                return null;

            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _serviceName },
                { _options.ActivityNameLabel, context.ActivityName },
                { _options.LogTypeLabel, GetLogTypeLabel<TLog>() }
            };

            return TrackInProgress(_compensateInProgress, tagList);
        }

        public static IDisposable TrackHandlerInProgress<TMessage>()
            where TMessage : class
        {
            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _serviceName },
                { _options.MessageTypeLabel, GetMessageTypeLabel<TMessage>() },
            };

            return TrackInProgress(_handlerInProgress, tagList);
        }

        static IDisposable TrackInProgress(Counter<long> counter, TagList tagList)
        {
            counter.Add(1, tagList);

            return new InProgressTracker(counter, tagList);
        }

        public static void TryConfigure(string serviceName, InstrumentationOptions options)
        {
            if (_isConfigured)
                return;

            _meter = new Meter("MassTransit", HostMetadataCache.Host.MassTransitVersion);

            _serviceName = serviceName;

            _options = options;

            // Counters

            _receiveTotal = _meter.CreateCounter<long>(options.ReceiveTotal, "ea", "Number of messages received");
            _receiveFaultTotal = _meter.CreateCounter<long>(options.ReceiveFaultTotal, "ea", "Number of messages receive faults");

            _consumeTotal = _meter.CreateCounter<long>(options.ConsumeTotal, "ea", "Number of messages consumed");
            _consumeFaultTotal = _meter.CreateCounter<long>(options.ConsumeFaultTotal, "ea", "Number of message consume faults");
            _consumeRetryTotal = _meter.CreateCounter<long>(options.ConsumeRetryTotal, "ea", "Number of message consume faults");

            _publishTotal = _meter.CreateCounter<long>(options.PublishTotal, "ea", "Number of messages published");
            _publishFaultTotal = _meter.CreateCounter<long>(options.PublishFaultTotal, "ea", "Number of message publish faults");

            _sendTotal = _meter.CreateCounter<long>(options.SendTotal, "ea", "Number of messages sent");
            _sendFaultTotal = _meter.CreateCounter<long>(options.SendFaultTotal, "ea", "Number of message send faults");

            _executeTotal = _meter.CreateCounter<long>(options.ActivityExecuteTotal, "ea", "Number of activities executed");
            _executeFaultTotal = _meter.CreateCounter<long>(options.ActivityExecuteFaultTotal, "ea", "Number of activity execution faults");

            _compensateTotal = _meter.CreateCounter<long>(options.ActivityCompensateTotal, "ea", "Number of activities compensated");
            _compensateFailureTotal = _meter.CreateCounter<long>(options.ActivityCompensateFailureTotal, "ea", "Number of activity compensation failures");

            // Gauges

            _receiveInProgress = _meter.CreateCounter<long>(options.ReceiveInProgress, "ea", "Number of messages being received");

            _handlerInProgress = _meter.CreateCounter<long>(options.HandlerInProgress, "ea", "Number of handlers in progress");

            _consumerInProgress = _meter.CreateCounter<long>(options.ConsumerInProgress, "ea", "Number of consumers in progress");

            _sagaInProgress = _meter.CreateCounter<long>(options.SagaInProgress, "ea", "Number of sagas in progress");

            _executeInProgress = _meter.CreateCounter<long>(options.ExecuteInProgress, "ea", "Number of activity executions in progress");

            _compensateInProgress = _meter.CreateCounter<long>(options.CompensateInProgress, "ea", "Number of activity compensations in progress");

            // Histograms

            _receiveDuration = _meter.CreateHistogram<double>(options.ReceiveDuration, "ms", "Elapsed time spent receiving a message, in seconds");

            _consumeDuration = _meter.CreateHistogram<double>(options.ConsumeDuration, "ms", "Elapsed time spent consuming a message, in seconds");

            _deliveryDuration = _meter.CreateHistogram<double>(options.DeliveryDuration, "ms",
                "Elapsed time between when the message was sent and when it was consumed, in seconds.");

            _executeDuration = _meter.CreateHistogram<double>(options.ActivityExecuteDuration, "ms", "Elapsed time spent executing an activity, in seconds");

            _compensateDuration = _meter.CreateHistogram<double>(options.ActivityCompensateDuration, "ms",
                "Elapsed time spent compensating an activity, in seconds");

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


        class InProgressTracker :
            IDisposable
        {
            readonly Counter<long> _counter;
            readonly TagList _tagList;

            public InProgressTracker(Counter<long> counter, TagList tagList)
            {
                _counter = counter;
                _tagList = tagList;
            }

            public void Dispose()
            {
                _counter.Add(-1, _tagList);
            }
        }
    }
}
