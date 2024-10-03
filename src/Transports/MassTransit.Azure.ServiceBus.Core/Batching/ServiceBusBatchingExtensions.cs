using System;

namespace MassTransit;

public static class ServiceBusBatchingExtensions
{
    public static void BatchBySession<TConsumer>(this IConsumerConfigurator<TConsumer> consumerConfigurator, Action<SessionBatchOptions> configure)
        where TConsumer : class
    {
        SessionBatchOptions sessionOptions = new();
        configure(sessionOptions);

        consumerConfigurator.Options<BatchOptions>(o =>
        {
            o.GroupBy<object, string>(e => e.SessionId())
              .SetConcurrencyLimit(sessionOptions.MaxConcurrentSessions)
              .SetMessageLimit(sessionOptions.MessageLimitPerSession)
              .SetTimeLimit(sessionOptions.TimeLimit)
              .SetTimeLimitStart(sessionOptions.TimeLimitStart)
              .OverrideConfigure((name, configurator) =>
              {
                  if (configurator is not IServiceBusReceiveEndpointConfigurator sb) throw new ArgumentException("Expecting IServiceBusReceiveEndpointConfigurator", nameof(configure));

                  sb.RequiresSession = true;

                  sb.MaxConcurrentSessions = sessionOptions.MaxConcurrentSessions;
                  sb.MaxConcurrentCallsPerSession = sessionOptions.MessageLimitPerSession;
                  sb.SessionIdleTimeout = sessionOptions.SessionIdleTimeout;
              });
        });

    }
}
