using System;

namespace MassTransit;

public record SessionBatchOptions
{
    /// <summary>
    /// The maximum number of messages in a single batch
    /// </summary>
    public int MessageLimitPerSession { get; set; } = 10;
    
    /// <summary>
    /// The maximum number of concurrent sessions
    /// </summary>
    public int MaxConcurrentSessions { get; set; } = 1;

    /// <summary>
    /// The timeout before a message session is abandoned
    /// </summary>
    public TimeSpan SessionIdleTimeout { get;  set; } = AzureServiceBusTransport.Defaults.SessionIdleTimeout;

    /// <summary>
    /// The maximum time to wait before delivering a partial batch
    /// </summary>
    public TimeSpan TimeLimit { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// The starting point for the <see cref="TimeLimit" />
    /// </summary>
    public BatchTimeLimitStart TimeLimitStart { get; set; } = BatchTimeLimitStart.FromFirst;

    /// <summary>
    /// Sets the maximum number of messages in a single batch
    /// </summary>
    /// <param name="limit">The message limit</param>
    public SessionBatchOptions SetMessageLimitPerSession(int limit)
    {
        MessageLimitPerSession = limit;
        return this;
    }

    /// <summary>
    /// Sets  maximum number of concurrent sessions
    /// </summary>
    /// <param name="limit">The maximum number of concurrent sessions</param>
    public SessionBatchOptions SetMaxConcurrentSessions(int limit)
    {
        MaxConcurrentSessions = limit;
        return this;
    }

    /// <summary>
    /// Sets the maximum time to wait for messages within a session before abandoning the session for another
    /// </summary>
    /// <param name="limit">The time limit</param>
    public SessionBatchOptions SetSessionIdleTimeout(TimeSpan limit)
    {
        SessionIdleTimeout = limit;
        return this;
    }

    /// <summary>
    /// Sets the maximum time to wait before delivering a partial batch
    /// </summary>
    /// <param name="limit">The time limit</param>
    public SessionBatchOptions SetTimeLimit(TimeSpan limit)
    {
        TimeLimit = limit;
        return this;
    }

    /// <summary>
    /// Sets the starting point for the <see cref="TimeLimit"/>
    /// </summary>
    /// <param name="timeLimitStart">The starting point</param>
    public SessionBatchOptions SetTimeLimitStart(BatchTimeLimitStart timeLimitStart)
    {
        TimeLimitStart = timeLimitStart;
        return this;
    }
}
