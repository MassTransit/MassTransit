# Using the circuit breaker

A circuit breaker is used to protect resources (remote, local, or otherwise) from being overloaded when
in a failure state. For example, a remote web site may be unavailable and calling that web site in a
message consumer takes 30-60 seconds to time out. By continuing to call the failing service, the service
may be unable to recover. A circuit breaker detects the repeated failures and trips, preventing further
calls to the service and giving it time to recover. Once the reset interval expires, calls are slowly allowed
back to the service. If it is still failing, the breaker remains open, and the timeout interval resets.
Once the service returns to healthy, calls flow normally as the breaker closes.

Read Martin Fowler's description of the pattern [here](http://martinfowler.com/bliki/CircuitBreaker.html).

To add the circuit breaker to a receive endpoint:

```csharp
cfg.ReceiveEndpoint(host, "customer_update_queue", e =>
{
    e.UseCircuitBreaker(cb =>
    {
        cb.TrackingPeriod = TimeSpan.FromMinutes(1);
        cb.TripThreshold = 15;
        cb.ActiveThreshold = 10;
        cb.ResetInterval = TimeSpan.FromMinutes(5);
    });
    // other configuration
});
```

There are four settings that can be adjusted on a circuit breaker.

### TrackingPeriod
The window of time before the success/failure counts are reset to zero. This is typically set to around
  one minute, but can be as high as necessary. More than ten seems really strange to me.

### TripThreshold
  This is a percentage, and is based on the ratio of successful to failed attempts. When set to 15, if the ratio
  exceeds 15%, the circuit breaker opens and remains open until the `ResetInterval` expires.

### ActiveThreshold
  This is the number of messages that must reach the circuit breaker in a tracking period before the circuit breaker
  can trip. If set to 10, the trip threshold is not evaluated until at least 10 messages have been received.

### ResetInterval
  The period of time between the circuit breaker trip and the first attempt to close the circuit breaker. Messages
  that reach the circuit breaker during the open period will immediately fail with the same exception that tripped
  the circuit breaker.
