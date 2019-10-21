# Using the rate limiter

A rate limiter is used to restrict the number of messages processed within a time period. The reason may be
that an API or service only accepts a certain number of calls per minute, and will delay any subsequent attempts
until the rate limiting period has expired.

<div class="alert alert-info">
<b>Note:</b>
    The rate limiter will delay message delivery until the rate limit expires, so it is best to avoid large time windows
    and keep the rate limits sane. Something like 1000 over 10 minutes is a bad idea, versus 100 over a minute. Try to
    adjust the values and see what works for you.
</div>

There are two modes that a rate limiter can operate, but only of them is currently supported (the other may come later).

To add a rate limiter to a receive endpoint:

```csharp
cfg.ReceiveEndpoint(host, "customer_update_queue", e =>
{
    e.UseRateLimit(1000, TimeSpan.FromSeconds(5));
    // other configuration
});
```

The two arguments supported by the rate limiter include:

### RateLimit
  The number of calls allowed in the time period.

### Interval
  The time interval before the message count is reset to zero.
