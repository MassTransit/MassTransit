namespace MassTransit;

using System;


public static class ServiceBusBatchingExtensions
{
    /// <summary>
    /// Configures <see cref="BatchOptions" /> for batching with Azure Service Bus sessions to ensure in-order processing of sessions
    /// - Sessions will be delivered as batches of up to <see cref="ServiceBusSessionBatchOptions.MessageLimitPerSession" />
    /// - Batches from up to <see cref="ServiceBusSessionBatchOptions.MaxConcurrentSessions" /> sessions will be processed concurrently
    /// Note: Consider max concurrency impacts of the SDK to avoid thread exhaustion
    /// - total number of concurrent calls = <see cref="ServiceBusSessionBatchOptions.MaxConcurrentSessions" /> *
    /// <see cref="ServiceBusSessionBatchOptions.MessageLimitPerSession" />
    /// </summary>
    public static void SetServiceBusSessionBatchOptions<TConsumer>(this IConsumerConfigurator<TConsumer> consumerConfigurator,
        Action<ServiceBusSessionBatchOptions> configure)
        where TConsumer : class
    {
        ServiceBusSessionBatchOptions sessionOptions = new();
        configure(sessionOptions);

        consumerConfigurator.Options<BatchOptions>(o =>
        {
            o.GroupBy<object, string>(e => e.SessionId())
                .SetConcurrencyLimit(sessionOptions.MaxConcurrentSessions)
                .SetMessageLimit(sessionOptions.MessageLimitPerSession)
                .SetTimeLimit(sessionOptions.TimeLimit)
                .SetTimeLimitStart(sessionOptions.TimeLimitStart)
                .SetConfigurationCallback((name, configurator) =>
                {
                    if (configurator is not IServiceBusEndpointConfigurator sb)
                        throw new ArgumentException("Expecting IServiceBusReceiveEndpointConfigurator", nameof(configurator));

                    sb.RequiresSession = true;

                    sb.MaxConcurrentSessions = sessionOptions.MaxConcurrentSessions;
                    sb.MaxConcurrentCallsPerSession = sessionOptions.MessageLimitPerSession;
                    sb.SessionIdleTimeout = sessionOptions.SessionIdleTimeout;

                    if (configurator.PrefetchCount != 0 && configurator.PrefetchCount < sessionOptions.MessageLimitPerSession)
                        configurator.PrefetchCount = sessionOptions.MessageLimitPerSession;
                });
        });
    }
}
