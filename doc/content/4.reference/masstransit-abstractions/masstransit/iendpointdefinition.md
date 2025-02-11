---

title: IEndpointDefinition

---

# IEndpointDefinition

Namespace: MassTransit

Defines an endpoint in a transport-independent way

```csharp
public interface IEndpointDefinition
```

## Properties

### **IsTemporary**

True if the endpoint is temporary, and should be removed when the bus/endpoint is stopped. Temporary queues
 should be configured as auto-delete, non-durable, express, whatever creates the least impact and fastest performance.

```csharp
public abstract bool IsTemporary { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PrefetchCount**

The number of messages to fetch in advance from the broker, if applicable. This should only be set when
 necessary, use the [IEndpointDefinition.ConcurrentMessageLimit](iendpointdefinition#concurrentmessagelimit) initially.

```csharp
public abstract Nullable<int> PrefetchCount { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConcurrentMessageLimit**

The maximum number of concurrent messages which can be delivered at any one time. This should be set by an
 endpoint before modifying the prefetch count. If this is specified, and [IEndpointDefinition.PrefetchCount](iendpointdefinition#prefetchcount) is left default,
 it will calculate an effective prefetch count automatically when supported.

```csharp
public abstract Nullable<int> ConcurrentMessageLimit { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConfigureConsumeTopology**

If true, configure the broker topology, which may include binding exchanges, subscribing to topics, etc.

```csharp
public abstract bool ConfigureConsumeTopology { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **GetEndpointName(IEndpointNameFormatter)**

Return the endpoint name for the consumer, using the specified formatter if necessary.

```csharp
string GetEndpointName(IEndpointNameFormatter formatter)
```

#### Parameters

`formatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Configure\<T\>(T, IRegistrationContext)**

Configure the endpoint, as provided by the transport-specific receive endpoint configurator

```csharp
void Configure<T>(T configurator, IRegistrationContext context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` T<br/>

`context` [IRegistrationContext](../masstransit/iregistrationcontext)<br/>
