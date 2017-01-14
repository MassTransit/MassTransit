# Retry policies

The retry policy can be specified at the bus level, as well as the endpoint and consumer level.
The policy closest to the exception is the policy that is used.

To configure the default retry policy for the entire bus.

```csharp
Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.UseRetry(retryConfig => retryConfig.None);
});
```

To configure the retry policy for a receive endpoint.

```csharp
Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("inbound", ep =>
    {
        ep.UseRetry(retryConfig => retryConfig.Immediate(5));
    });
});
```

To configure the retry policy for a specific consumer on an endpoint.

```csharp
Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("inbound", ep =>
    {
        ep.Consumer<MyConsumer>(consumerCfg =>
        {
            consumerCfg.UseRetry(retryConfig => 
                retryConfig.Interval(10, TimeSpan.FromMilliseconds(200)));
        });
    });
});
```

### Available retry policies

* None
* Immediate
* Intervals
* Exponential
* Incremental

## Retry filters

Sometimes you do not want to always retry, but instead only retry when some specific exception
is thrown and fail for all other exceptions.

To implement this, you can use an exception filter.

### Available filters

* Handle
* Ignore

Sample:
```csharp
Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("inbound", ep =>
    {
        ep.UseRetry(retryConfig => 
        {
            retryConfig.Immediate(5);
            retryConfig.Handle(x => x.Message.Contains("SQL"));
        });
        ep.Consumer<MyConsumer>(consumerCfg =>
            consumerCfg.UseRetry(retryConfig => 
            {
                retryConfig.Interval(10, TimeSpan.FromMilliseconds(200));
                retryConfig.Ignore<ArgumentNullException>();
            });
        );
    });
});
```