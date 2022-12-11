# Consumers

To understand consumers and how to create one, refer to the [Consumers](/documentation/concepts/consumers) section.

## Adding Consumers

Consumers are added inside the `AddMassTransit` configuration using any of the following methods.

```csharp
AddConsumer<MyConsumer>();
```

Adds a consumer.

```csharp
AddConsumer<MyConsumer, MyConsumerDefinition>();
```

Adds a consumer with a matching consumer definition.

```csharp
AddConsumer<MyConsumer, MyConsumerDefinition>(cfg =>
{
    cfg.ConcurrentMessageLimit = 8;
});
```

Adds a consumer with a matching consumer definition and configures the consumer pipeline.

```csharp
AddConsumer(typeof(MyConsumer));
```

Adds a consumer by type.

```csharp
AddConsumer(typeof(MyConsumer), typeof(MyConsumerDefinition));
```

Adds a consumer with a matching consumer definition by type.

```csharp
void AddConsumers(params Type[] types);
```

Adds the specified consumers and consumer definitions. When consumer definitions are included they will be added with the matching consumer type.

```csharp
void AddConsumers(params Assembly[] assemblies);
```

Adds all consumers and consumer definitions in the specified an assembly or assemblies. 

```csharp
void AddConsumers(Func<Type, bool> filter, params Assembly[] assemblies);
```

Adds the consumers and any matching consumer definitions in the specified an assembly or assemblies that pass the filter. The filter is only called for consumer types.

## Batch Options

```csharp
AddConsumer<MyBatchConsumer>(cfg =>
{
    cfg.Options<BatchOptions>(options => options
        .SetMessageLimit(100)
        .SetTimeLimit(s: 1)
        .SetTimeLimitStart(BatchTimeLimitStart.FromLast)
        .GroupBy<MyMessage>(x => x.CustomerId)
        .SetConcurrencyLimit(10));
});
```

Adds a batch consumer and configures the batch options.

## Job Options

```csharp
AddConsumer<MyJobConsumer>(cfg =>
{
    cfg.Options<JobOptions<MyJob>>(options => options
        .SetMessageLimit(100)
        .SetTimeLimit(s: 1)
        .SetTimeLimitStart(BatchTimeLimitStart.FromLast)
        .GroupBy<MyMessage>(x => x.CustomerId)
        .SetConcurrencyLimit(10));
});
```

Adds a job consumer and configures the job options.


## Configuring Consumers

Consumers are automatically configured when `ConfigureEndpoints` is called, which is highly recommended. The endpoint configuration can be mostly customized using either a consumer definition or by specifying the endpoint configuration inline.

To manually configure a consumer on a receive endpoint, use one of the following methods.

::card{icon="icon-park-outline:info"}
#title
Order Matters
#description
Manually configured receive endpoints should be configured **before** calling _ConfigureEndpoints_.
::

::alert{type="warning"}
Manually configured receive endpoints should be configured **before** calling _ConfigureEndpoints_.
::

```csharp
cfg.ReceiveEndpoint("manually-configured", e =>
{
    // configure endpoint-specific settings first
    e.SomeEndpointSetting = someValue;
    
    // configure any required middleware components next
    e.UseMessageRetry(r => r.Interval(5, 1000));
    
    // configure the consumer last
    e.ConfigureConsumer<MyConsumer>(context);
});

// configure any remaining consumers, sagas, etc.
cfg.ConfigureEndpoints(context);
```

#### Configuration Methods

```csharp
ConfigureConsumer<T>(context);
```

Configures the consumer on the receive endpoint. 

```csharp
ConfigureConsumer<T>(context, consumer => 
{
    // configure consumer-specific middleware
});
```

Configures the consumer on the receive endpoint and applies the additional consumer configuration to the consumer pipeline. 

```csharp
ConfigureConsumers(context);
```

Configures all consumers that haven't been configured on the receive endpoint.

