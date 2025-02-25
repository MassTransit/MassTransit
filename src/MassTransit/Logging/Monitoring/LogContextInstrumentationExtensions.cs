namespace MassTransit.Logging
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Diagnostics.Metrics;
    using System.Linq;
    using System.Text;
    using Courier.Contracts;
    using Metadata;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Middleware;
    using Monitoring;
    using Transports;


    public static class LogContextInstrumentationExtensions
    {
        static readonly ConcurrentDictionary<string, string> _labelCache = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        static bool _isConfigured;
        static Counter<long> _receiveTotal;
        static Counter<long> _receiveFaultTotal;
        static Counter<long> _receiveInProgress;
        static Counter<long> _consumeTotal;
        static Counter<long> _consumeFaultTotal;
        static Counter<long> _consumeRetryTotal;
        static Counter<long> _sagaTotal;
        static Counter<long> _sagaFaultTotal;
        static Counter<long> _sendTotal;
        static Counter<long> _sendFaultTotal;
        static Counter<long> _executeTotal;
        static Counter<long> _executeFaultTotal;
        static Counter<long> _compensateTotal;
        static Counter<long> _compensateFaultTotal;
        static Counter<long> _consumerInProgress;
        static Counter<long> _handlerTotal;
        static Counter<long> _handlerFaultTotal;
        static Counter<long> _handlerInProgress;
        static Counter<long> _sagaInProgress;
        static Counter<long> _executeInProgress;
        static Counter<long> _compensateInProgress;
        static Counter<long> _outboxSendTotal;
        static Counter<long> _outboxSendFaultTotal;
        static Counter<long> _outboxDeliveryTotal;
        static Counter<long> _outboxDeliveryFaultTotal;
        static Histogram<double> _receiveDuration;
        static Histogram<double> _consumeDuration;
        static Histogram<double> _handlerDuration;
        static Histogram<double> _sagaDuration;
        static Histogram<double> _deliveryDuration;
        static Histogram<double> _executeDuration;
        static Histogram<double> _compensateDuration;

        static readonly char[] _delimiters = { '<', '>' };

        static Meter _meter;
        static InstrumentationOptions _options;

        public static StartedInstrument? StartReceiveInstrument(this ILogContext logContext, ReceiveContext context)
        {
            if (!_isConfigured || !_receiveTotal.Enabled)
                return null;

            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _options.ServiceName },
                { _options.EndpointLabel, GetEndpointLabel(context.InputAddress) }
            };

            AddCustomTags(ref tagList, context);

            _receiveTotal.Add(1, tagList);
            _receiveInProgress.Add(1, tagList);

            return new StartedInstrument(exception =>
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);
                _receiveFaultTotal.Add(1, tagList);
            }, () =>
            {
                _receiveInProgress.Add(-1, tagList);
                _receiveDuration.Record(context.ElapsedTime.TotalMilliseconds, tagList);
            });
        }

        public static StartedInstrument? StartHandlerInstrument<TMessage>(this ILogContext logContext, ConsumeContext<TMessage> context,
            Stopwatch stopwatch)
            where TMessage : class
        {
            if (!_isConfigured || !_handlerTotal.Enabled)
                return null;

            var messageTypeLabel = GetMessageTypeLabel<TMessage>();
            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _options.ServiceName },
                { _options.EndpointLabel, GetEndpointLabel(context.ReceiveContext.InputAddress) },
                { _options.MessageTypeLabel, messageTypeLabel },
                { _options.ConsumerTypeLabel, GetConsumerTypeLabel<MessageHandler<TMessage>, TMessage>(messageTypeLabel) }
            };

            AddCustomTags(ref tagList, context);

            _handlerTotal.Add(1, tagList);
            _handlerInProgress.Add(1, tagList);

            return new StartedInstrument(exception =>
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);
                _handlerFaultTotal.Add(1, tagList);
            }, () =>
            {
                _handlerInProgress.Add(-1, tagList);
                _handlerDuration.Record(stopwatch.ElapsedMilliseconds, tagList);
            });
        }

        public static StartedInstrument? StartSagaInstrument<TSaga, T>(this ILogContext logContext, SagaConsumeContext<TSaga, T> context)
            where T : class
            where TSaga : class, ISaga
        {
            if (!_isConfigured || !_sagaTotal.Enabled)
                return null;

            var messageTypeLabel = GetMessageTypeLabel<T>();
            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _options.ServiceName },
                { _options.EndpointLabel, GetEndpointLabel(context.ReceiveContext.InputAddress) },
                { _options.MessageTypeLabel, messageTypeLabel },
                { _options.ConsumerTypeLabel, GetConsumerTypeLabel<TSaga, T>(messageTypeLabel) }
            };

            AddCustomTags(ref tagList, context);

            _sagaTotal.Add(1, tagList);
            _sagaInProgress.Add(1, tagList);

            return new StartedInstrument(exception =>
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);
                _sagaFaultTotal.Add(1, tagList);
            }, () =>
            {
                _sagaInProgress.Add(-1, tagList);
                _sagaDuration.Record(context.ReceiveContext.ElapsedTime.TotalMilliseconds, tagList);
            });
        }

        public static StartedInstrument? StartSagaStateMachineInstrument<TSaga, T>(this ILogContext logContext, BehaviorContext<TSaga, T> context)
            where T : class
            where TSaga : class, SagaStateMachineInstance
        {
            if (!_isConfigured || !_sagaTotal.Enabled)
                return null;

            var messageTypeLabel = GetMessageTypeLabel<T>();
            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _options.ServiceName },
                { _options.EndpointLabel, GetEndpointLabel(context.ReceiveContext.InputAddress) },
                { _options.MessageTypeLabel, messageTypeLabel },
                { _options.ConsumerTypeLabel, GetConsumerTypeLabel<TSaga, T>(messageTypeLabel) }
            };

            AddCustomTags(ref tagList, context);

            _sagaTotal.Add(1, tagList);
            _sagaInProgress.Add(1, tagList);

            return new StartedInstrument(exception =>
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);
                _sagaFaultTotal.Add(1, tagList);
            }, () =>
            {
                _sagaInProgress.Add(-1, tagList);
                _sagaDuration.Record(context.ReceiveContext.ElapsedTime.TotalMilliseconds, tagList);
            });
        }

        public static StartedInstrument? StartConsumeInstrument<TConsumer, T>(this ILogContext logContext, ConsumeContext<T> context, Stopwatch timer)
            where T : class
        {
            if (!_isConfigured || !_consumeTotal.Enabled)
                return null;

            var messageTypeLabel = GetMessageTypeLabel<T>();
            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _options.ServiceName },
                { _options.EndpointLabel, GetEndpointLabel(context.ReceiveContext.InputAddress) },
                { _options.MessageTypeLabel, messageTypeLabel },
                { _options.ConsumerTypeLabel, GetConsumerTypeLabel<TConsumer, T>(messageTypeLabel) }
            };

            AddCustomTags(ref tagList, context);

            _consumeTotal.Add(1, tagList);
            _consumerInProgress.Add(1, tagList);

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

            return new StartedInstrument(exception =>
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);
                _consumeFaultTotal.Add(1, tagList);
            }, () =>
            {
                _consumerInProgress.Add(-1, tagList);
                _consumeDuration.Record(timer.ElapsedMilliseconds, tagList);
            });
        }

        public static StartedInstrument? StartActivityExecuteInstrument<TActivity, TArguments>(this ILogContext logContext,
            ConsumeContext<RoutingSlip> context, Stopwatch timer)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            if (!_isConfigured || !_executeTotal.Enabled)
                return null;

            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _options.ServiceName },
                { _options.EndpointLabel, GetEndpointLabel(context.ReceiveContext.InputAddress) },
                { _options.ActivityNameLabel, GetActivityTypeLabel<TActivity>() },
                { _options.ArgumentTypeLabel, GetArgumentTypeLabel<TArguments>() }
            };

            AddCustomTags(ref tagList, context);

            _executeTotal.Add(1, tagList);
            _executeInProgress.Add(1, tagList);

            return new StartedInstrument(exception =>
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);
                _executeFaultTotal.Add(1, tagList);
            }, () =>
            {
                _executeInProgress.Add(-1, tagList);
                _executeDuration.Record(timer.ElapsedMilliseconds, tagList);
            });
        }

        public static StartedInstrument? StartActivityCompensateInstrument<TActivity, TLog>(this ILogContext logContext,
            ConsumeContext<RoutingSlip> context, Stopwatch timer)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            if (!_isConfigured || !_compensateTotal.Enabled)
                return null;

            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _options.ServiceName },
                { _options.EndpointLabel, GetEndpointLabel(context.ReceiveContext.InputAddress) },
                { _options.ActivityNameLabel, GetActivityTypeLabel<TActivity>() },
                { _options.LogTypeLabel, GetLogTypeLabel<TLog>() }
            };

            AddCustomTags(ref tagList, context);

            _compensateTotal.Add(1, tagList);
            _compensateInProgress.Add(1, tagList);

            return new StartedInstrument(exception =>
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);
                _compensateFaultTotal.Add(1, tagList);
            }, () =>
            {
                _compensateInProgress.Add(-1, tagList);
                _compensateDuration.Record(timer.ElapsedMilliseconds, tagList);
            });
        }

        public static StartedInstrument? StartSendInstrument<T>(this ILogContext logContext, SendTransportContext transportContext, SendContext<T> context)
            where T : class
        {
            if (!_isConfigured || !_sendTotal.Enabled)
                return null;

            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _options.ServiceName },
                { _options.EndpointLabel, GetEndpointLabel(context.DestinationAddress) },
                { _options.MessageTypeLabel, GetMessageTypeLabel<T>() }
            };

            AddCustomTags(ref tagList, context);

            _sendTotal.Add(1, tagList);

            return new StartedInstrument(exception =>
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);
                _sendFaultTotal.Add(1, tagList);
            });
        }

        public static StartedInstrument? StartOutboxSendInstrument<T>(this ILogContext logContext, SendContext<T> context)
            where T : class
        {
            if (!_isConfigured || !_outboxSendTotal.Enabled)
                return null;

            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _options.ServiceName },
                { _options.EndpointLabel, GetEndpointLabel(context.DestinationAddress) },
                { _options.MessageTypeLabel, GetMessageTypeLabel<T>() }
            };

            AddCustomTags(ref tagList, context);

            _outboxSendTotal.Add(1, tagList);

            return new StartedInstrument(exception =>
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);
                _outboxSendFaultTotal.Add(1, tagList);
            });
        }

        public static StartedInstrument? StartOutboxDeliveryInstrument(this ILogContext logContext, OutboxMessageContext context)
        {
            if (!_isConfigured || !_outboxDeliveryTotal.Enabled)
                return null;

            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _options.ServiceName },
                { _options.EndpointLabel, GetEndpointLabel(context.DestinationAddress) }
            };

            _outboxDeliveryTotal.Add(1, tagList);

            return new StartedInstrument(exception =>
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);
                _outboxDeliveryFaultTotal.Add(1, tagList);
            });
        }

        public static StartedInstrument? StartOutboxDeliveryInstrument(this ILogContext logContext,
            OutboxConsumeContext consumeContext, OutboxMessageContext context)
        {
            if (!_isConfigured || !_outboxDeliveryTotal.Enabled)
                return null;

            var tagList = new TagList
            {
                { _options.ServiceNameLabel, _options.ServiceName },
                { _options.EndpointLabel, GetEndpointLabel(context.DestinationAddress) }
            };

            AddCustomTags(ref tagList, consumeContext);

            _outboxDeliveryTotal.Add(1, tagList);

            return new StartedInstrument(exception =>
            {
                tagList.Add(_options.ExceptionTypeLabel, exception.GetType().Name);
                _outboxDeliveryFaultTotal.Add(1, tagList);
            });
        }

        public static void TryConfigure(IServiceProvider provider)
        {
            if (_isConfigured)
                return;

            var instrumentationOptions = provider.GetRequiredService<IOptions<InstrumentationOptions>>().Value;
        #if NET8_0_OR_GREATER
            var meterFactory = provider.GetService<IMeterFactory>();
            if (meterFactory == null)
            {
                TryConfigure(instrumentationOptions);
                return;
            }

            var meter = meterFactory.Create(new MeterOptions(InstrumentationOptions.MeterName) { Version = HostMetadataCache.Host.MassTransitVersion });
            Configure(meter, instrumentationOptions);
        #else
            TryConfigure(instrumentationOptions);
        #endif
        }

        public static void TryConfigure(InstrumentationOptions options)
        {
            if (_isConfigured)
                return;

            // We have to dispose manually created meter to flush instruments, some day...
            Configure(new Meter(InstrumentationOptions.MeterName, HostMetadataCache.Host.MassTransitVersion), options);
        }

        static void Configure(Meter meter, InstrumentationOptions options)
        {
            _options = options;
            _meter = meter;

            // Counters

            _receiveTotal = _meter.CreateCounter<long>(options.ReceiveTotal, "ea", "Number of messages received");
            _receiveFaultTotal = _meter.CreateCounter<long>(options.ReceiveFaultTotal, "ea", "Number of messages receive faults");

            _consumeTotal = _meter.CreateCounter<long>(options.ConsumeTotal, "ea", "Number of messages consumed");
            _consumeFaultTotal = _meter.CreateCounter<long>(options.ConsumeFaultTotal, "ea", "Number of message consume faults");
            _consumeRetryTotal = _meter.CreateCounter<long>(options.ConsumeRetryTotal, "ea", "Number of message consume retries");

            _sagaTotal = _meter.CreateCounter<long>(options.SagaTotal, "ea", "Number of sagas executed");
            _sagaFaultTotal = _meter.CreateCounter<long>(options.SagaFaultTotal, "ea", "Number of sagas faults");

            _handlerTotal = _meter.CreateCounter<long>(options.HandlerTotal, "ea", "Number of messages handled");
            _handlerFaultTotal = _meter.CreateCounter<long>(options.HandlerFaultTotal, "ea", "Number of message handler faults");

            _sendTotal = _meter.CreateCounter<long>(options.SendTotal, "ea", "Number of messages sent");
            _sendFaultTotal = _meter.CreateCounter<long>(options.SendFaultTotal, "ea", "Number of message send faults");

            _outboxSendTotal = _meter.CreateCounter<long>(options.OutboxSendTotal, "ea", "Number of messages sent to outbox");
            _outboxSendFaultTotal = _meter.CreateCounter<long>(options.OutboxSendFaultTotal, "ea", "Number of message send to outbox faults");

            _executeTotal = _meter.CreateCounter<long>(options.ActivityExecuteTotal, "ea", "Number of activities executed");
            _executeFaultTotal = _meter.CreateCounter<long>(options.ActivityExecuteFaultTotal, "ea", "Number of activity execution faults");

            _compensateTotal = _meter.CreateCounter<long>(options.ActivityCompensateTotal, "ea", "Number of activities compensated");
            _compensateFaultTotal = _meter.CreateCounter<long>(options.ActivityCompensateFailureTotal, "ea", "Number of activity compensation failures");

            _outboxDeliveryTotal = _meter.CreateCounter<long>(options.OutboxDeliveryTotal, "ea", "Number of outbox delivery messages executed");
            _outboxDeliveryFaultTotal = _meter.CreateCounter<long>(options.OutboxDeliveryFaultTotal, "ea", "Number of outbox delivery message failures");

            // Gauges

            _receiveInProgress = _meter.CreateCounter<long>(options.ReceiveInProgress, "ea", "Number of messages being received");

            _handlerInProgress = _meter.CreateCounter<long>(options.HandlerInProgress, "ea", "Number of handlers in progress");

            _consumerInProgress = _meter.CreateCounter<long>(options.ConsumerInProgress, "ea", "Number of consumers in progress");

            _sagaInProgress = _meter.CreateCounter<long>(options.SagaInProgress, "ea", "Number of sagas in progress");

            _executeInProgress = _meter.CreateCounter<long>(options.ExecuteInProgress, "ea", "Number of activity executions in progress");

            _compensateInProgress = _meter.CreateCounter<long>(options.CompensateInProgress, "ea", "Number of activity compensations in progress");

            // Histograms

            _receiveDuration = _meter.CreateHistogram<double>(options.ReceiveDuration, "ms", "Elapsed time spent receiving a message, in millis");

            _consumeDuration = _meter.CreateHistogram<double>(options.ConsumeDuration, "ms", "Elapsed time spent consuming a message, in millis");

            _sagaDuration = _meter.CreateHistogram<double>(options.SagaDuration, "ms", "Elapsed time spent saga processing a message, in millis");

            _handlerDuration = _meter.CreateHistogram<double>(options.HandlerDuration, "ms", "Elapsed time spent handler processing a message, in millis");

            _deliveryDuration = _meter.CreateHistogram<double>(options.DeliveryDuration, "ms",
                "Elapsed time between when the message was sent and when it was consumed, in millis.");

            _executeDuration = _meter.CreateHistogram<double>(options.ActivityExecuteDuration, "ms", "Elapsed time spent executing an activity, in millis");

            _compensateDuration = _meter.CreateHistogram<double>(options.ActivityCompensateDuration, "ms",
                "Elapsed time spent compensating an activity, in millis");

            _isConfigured = true;
        }

        static void AddCustomTags(ref TagList tags, PipeContext pipeContext)
        {
            if (pipeContext.TryGetPayload<MetricsContext>(out var metricsContext))
                metricsContext.Populate(ref tags);
        }

        static string GetConsumerTypeLabel<TConsumer, TMessage>(string messageLabel)
        {
            return _labelCache.GetOrAdd(TypeCache<TConsumer>.ShortName, type =>
            {
                if (type.StartsWith("MassTransit.MessageHandler<"))
                    return "Handler";

                var genericMessageType = "<" + TypeCache<TMessage>.ShortName + ">";
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

        static string GetActivityTypeLabel<TActivity>()
        {
            return _labelCache.GetOrAdd(TypeCache<TActivity>.ShortName, type => FormatTypeName(new StringBuilder(), typeof(TActivity)).Replace("Activity", ""));
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

            if (type.IsGenericType)
            {
                var name = type.GetGenericTypeDefinition().Name;

                //remove `1
                var index = name.IndexOf('`');
                if (index > 0)
                    name = name.Remove(index);

                sb.Append(name);
                sb.Append('_');
                Type[] arguments = type.GenericTypeArguments;
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
