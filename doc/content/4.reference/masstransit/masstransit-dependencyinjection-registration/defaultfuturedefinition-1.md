---

title: DefaultFutureDefinition<TFuture>

---

# DefaultFutureDefinition\<TFuture\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class DefaultFutureDefinition<TFuture> : FutureDefinition<TFuture>, IFutureDefinition<TFuture>, IFutureDefinition, IDefinition
```

#### Type Parameters

`TFuture`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureDefinition\<TFuture\>](../../masstransit-abstractions/masstransit/futuredefinition-1) → [DefaultFutureDefinition\<TFuture\>](../masstransit-dependencyinjection-registration/defaultfuturedefinition-1)<br/>
Implements [IFutureDefinition\<TFuture\>](../../masstransit-abstractions/masstransit/ifuturedefinition-1), [IFutureDefinition](../../masstransit-abstractions/masstransit/ifuturedefinition), [IDefinition](../../masstransit-abstractions/masstransit/idefinition)

## Properties

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

### **DefaultFutureDefinition()**

```csharp
public DefaultFutureDefinition()
```

## Methods

### **ConfigureSaga(IReceiveEndpointConfigurator, ISagaConfigurator\<FutureState\>, IRegistrationContext)**

```csharp
protected void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FutureState> sagaConfigurator, IRegistrationContext context)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`sagaConfigurator` [ISagaConfigurator\<FutureState\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
