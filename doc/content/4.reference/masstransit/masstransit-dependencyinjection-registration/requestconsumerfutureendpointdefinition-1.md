---

title: RequestConsumerFutureEndpointDefinition<TFuture>

---

# RequestConsumerFutureEndpointDefinition\<TFuture\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class RequestConsumerFutureEndpointDefinition<TFuture> : IEndpointDefinition<TFuture>, IEndpointDefinition
```

#### Type Parameters

`TFuture`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestConsumerFutureEndpointDefinition\<TFuture\>](../masstransit-dependencyinjection-registration/requestconsumerfutureendpointdefinition-1)<br/>
Implements [IEndpointDefinition\<TFuture\>](../../masstransit-abstractions/masstransit/iendpointdefinition-1), [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)

## Properties

### **ConfigureConsumeTopology**

```csharp
public bool ConfigureConsumeTopology { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsTemporary**

```csharp
public bool IsTemporary { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PrefetchCount**

```csharp
public Nullable<int> PrefetchCount { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **RequestConsumerFutureEndpointDefinition(IDefinition, IConsumerDefinition)**

```csharp
public RequestConsumerFutureEndpointDefinition(IDefinition definition, IConsumerDefinition consumerDefinition)
```

#### Parameters

`definition` [IDefinition](../../masstransit-abstractions/masstransit/idefinition)<br/>

`consumerDefinition` [IConsumerDefinition](../../masstransit-abstractions/masstransit/iconsumerdefinition)<br/>

## Methods

### **Configure\<T\>(T, IRegistrationContext)**

```csharp
public void Configure<T>(T configurator, IRegistrationContext context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` T<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **GetEndpointName(IEndpointNameFormatter)**

```csharp
public string GetEndpointName(IEndpointNameFormatter formatter)
```

#### Parameters

`formatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
