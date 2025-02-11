---

title: FutureDefinition<TFuture>

---

# FutureDefinition\<TFuture\>

Namespace: MassTransit

A future definition defines the configuration for a future, which can be used by the automatic registration code to
 configure the consumer on a receive endpoint.

```csharp
public class FutureDefinition<TFuture> : IFutureDefinition<TFuture>, IFutureDefinition, IDefinition
```

#### Type Parameters

`TFuture`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureDefinition\<TFuture\>](../masstransit/futuredefinition-1)<br/>
Implements [IFutureDefinition\<TFuture\>](../masstransit/ifuturedefinition-1), [IFutureDefinition](../masstransit/ifuturedefinition), [IDefinition](../masstransit/idefinition)

## Properties

### **EndpointDefinition**

```csharp
public IEndpointDefinition<TFuture> EndpointDefinition { get; set; }
```

#### Property Value

[IEndpointDefinition\<TFuture\>](../masstransit/iendpointdefinition-1)<br/>

### **ConcurrentMessageLimit**

Set the concurrent message limit for the saga, which limits how many saga instances are able to concurrently
 consume messages.

```csharp
public Nullable<int> ConcurrentMessageLimit { get; protected set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **ConfigureSaga(IReceiveEndpointConfigurator, ISagaConfigurator\<FutureState\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Called when configuring the saga on the endpoint. Configuration only applies to this saga, and does not apply to
 the endpoint.

```csharp
protected void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FutureState> sagaConfigurator)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The receive endpoint configurator for the consumer

`sagaConfigurator` [ISagaConfigurator\<FutureState\>](../masstransit/isagaconfigurator-1)<br/>
The saga configurator

### **ConfigureSaga(IReceiveEndpointConfigurator, ISagaConfigurator\<FutureState\>, IRegistrationContext)**

Called when configuring the saga on the endpoint. Configuration only applies to this saga, and does not apply to
 the endpoint.

```csharp
protected void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FutureState> sagaConfigurator, IRegistrationContext context)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The receive endpoint configurator for the consumer

`sagaConfigurator` [ISagaConfigurator\<FutureState\>](../masstransit/isagaconfigurator-1)<br/>
The saga configurator

`context` [IRegistrationContext](../masstransit/iregistrationcontext)<br/>

### **Endpoint(Action\<IEndpointRegistrationConfigurator\>)**

Configure the saga endpoint

```csharp
protected void Endpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
