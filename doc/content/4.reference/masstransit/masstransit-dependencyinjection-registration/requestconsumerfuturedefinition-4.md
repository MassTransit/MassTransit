---

title: RequestConsumerFutureDefinition<TFuture, TConsumer, TRequest, TResponse>

---

# RequestConsumerFutureDefinition\<TFuture, TConsumer, TRequest, TResponse\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class RequestConsumerFutureDefinition<TFuture, TConsumer, TRequest, TResponse> : RequestConsumerFutureDefinition<TFuture, TConsumer, TRequest, TResponse, Fault<TRequest>>, IFutureDefinition<TFuture>, IFutureDefinition, IDefinition, IFutureRequestDefinition<TRequest>
```

#### Type Parameters

`TFuture`<br/>

`TConsumer`<br/>

`TRequest`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureDefinition\<TFuture\>](../../masstransit-abstractions/masstransit/futuredefinition-1) → [RequestConsumerFutureDefinition\<TFuture, TConsumer, TRequest, TResponse, Fault\<TRequest\>\>](../masstransit-dependencyinjection-registration/requestconsumerfuturedefinition-5) → [RequestConsumerFutureDefinition\<TFuture, TConsumer, TRequest, TResponse\>](../masstransit-dependencyinjection-registration/requestconsumerfuturedefinition-4)<br/>
Implements [IFutureDefinition\<TFuture\>](../../masstransit-abstractions/masstransit/ifuturedefinition-1), [IFutureDefinition](../../masstransit-abstractions/masstransit/ifuturedefinition), [IDefinition](../../masstransit-abstractions/masstransit/idefinition), [IFutureRequestDefinition\<TRequest\>](../masstransit/ifuturerequestdefinition-1)

## Properties

### **RequestAddress**

```csharp
public Uri RequestAddress { get; }
```

#### Property Value

Uri<br/>

### **EndpointDefinition**

```csharp
public IEndpointDefinition<TFuture> EndpointDefinition { get; set; }
```

#### Property Value

[IEndpointDefinition\<TFuture\>](../../masstransit-abstractions/masstransit/iendpointdefinition-1)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; protected set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **RequestConsumerFutureDefinition(IConsumerDefinition\<TConsumer\>)**

```csharp
public RequestConsumerFutureDefinition(IConsumerDefinition<TConsumer> consumerDefinition)
```

#### Parameters

`consumerDefinition` [IConsumerDefinition\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerdefinition-1)<br/>
