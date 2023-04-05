# Sagas

To understand sagas and how to create one, refer to the [Sagas](/documentation/concepts/sagas) section.

## Adding Sagas

Sagas are added inside the `AddMassTransit` configuration using any of the following methods.

```csharp
AddSaga<MySaga>();
```

Adds a saga.

```csharp
AddSaga<MySaga, MySagaDefinition>();
```

Adds a saga with a matching saga definition.

```csharp
AddSaga<MySaga, MySagaDefinition>(cfg =>
{
    cfg.ConcurrentMessageLimit = 8;
});
```

Adds a saga with a matching saga definition and configures the saga pipeline.

```csharp
AddSaga(typeof(MySaga));
```

Adds a saga by type.

```csharp
AddSaga(typeof(MySaga), typeof(MySagaDefinition));
```

Adds a saga with a matching saga definition by type.

```csharp
AddSagas(params Type[] types);
```

Adds the specified sagas and saga definitions. When saga definitions are included they will be added with the matching saga type.

```csharp
AddSagas(params Assembly[] assemblies);
```

Adds all sagas and saga definitions in the specified an assembly or assemblies. 

```csharp
AddSagas(Func<Type, bool> filter, params Assembly[] assemblies);
```

Adds the sagas and any matching saga definitions in the specified an assembly or assemblies that pass the filter. The filter is only called for saga types.


## Configuring Sagas

Sagas are automatically configured when `ConfigureEndpoints` is called, which is highly recommended. The endpoint configuration can be mostly customized using either a saga definition or by specifying the endpoint configuration inline.

To manually configure a saga on a receive endpoint, use one of the following methods.

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
    
    // configure the saga last
    e.ConfigureSaga<MySaga>(context);
});

// configure any remaining consumers, sagas, etc.
cfg.ConfigureEndpoints(context);
```

#### Configuration Methods

```csharp
ConfigureSaga<T>(context);
```

Configures the saga on the receive endpoint. 

```csharp
ConfigureSaga<T>(context, saga => 
{
    // configure saga-specific middleware
});
```

Configures the saga on the receive endpoint and applies the additional saga configuration to the saga pipeline. 

```csharp
ConfigureSagas(context);
```

Configures all sagas that haven't been configured on the receive endpoint.
