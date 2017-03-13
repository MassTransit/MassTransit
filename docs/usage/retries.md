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

* Handle - only process specified exceptions
* Ignore - ignore specified exceptions

Filters can be combined to narrow down a set of exception types that you need to apply the retry policy to.

Both filters exist in two version:

1. Generic version `Handle<T>` and `Ignore<T>` where `T` must be derivate of 
`System.Exception`. With no argument, all exceptions of specified type will be either handled or ignored.
You can also specify a function argument that will filter exceptions further based on other parameters.

1. Non-generic version that needs one or more exception types as parameters. No further filtering
is possible if this version is used.

You can use multiple calls to these methods to specify filters for multiple exception types:

```csharp
cfg.UseRetry(r => 
{
    c.Handle<ArgumentNullException>();
    c.Ignore(typeof(InvalidOperationException), typeof(InvalidCastException));
    c.Ignore<ArgumentException>(t => t.ParamName == "orderTotal");
});
```

**Sample:**
```csharp
Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("inbound", ep =>
    {
        ep.UseRetry(retryConfig => 
        {
            retryConfig.Immediate(5);
            retryConfig.Handle<DataException>(x => x.Message.Contains("SQL"));
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