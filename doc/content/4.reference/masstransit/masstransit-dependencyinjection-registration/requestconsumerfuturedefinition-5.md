---

title: RequestConsumerFutureDefinition<TFuture, TConsumer, TRequest, TResponse, TFault>

---

# RequestConsumerFutureDefinition\<TFuture, TConsumer, TRequest, TResponse, TFault\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class RequestConsumerFutureDefinition<TFuture, TConsumer, TRequest, TResponse, TFault> : FutureDefinition<TFuture>, IFutureDefinition<TFuture>, IFutureDefinition, IDefinition, IFutureRequestDefinition<TRequest>
```

#### Type Parameters

`TFuture`<br/>

`TConsumer`<br/>

`TRequest`<br/>

`TResponse`<br/>

`TFault`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureDefinition\<TFuture\>](../../masstransit-abstractions/masstransit/futuredefinition-1) → [RequestConsumerFutureDefinition\<TFuture, TConsumer, TRequest, TResponse, TFault\>](../masstransit-dependencyinjection-registration/requestconsumerfuturedefinition-5)<br/>
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

## Methods

### **ConfigureSaga(IReceiveEndpointConfigurator, ISagaConfigurator\<FutureState\>, IRegistrationContext)**

```csharp
protected void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FutureState> sagaConfigurator, IRegistrationContext context)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`sagaConfigurator` [ISagaConfigurator\<FutureState\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
