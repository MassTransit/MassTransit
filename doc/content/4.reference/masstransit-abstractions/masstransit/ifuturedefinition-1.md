---

title: IFutureDefinition<TFuture>

---

# IFutureDefinition\<TFuture\>

Namespace: MassTransit

```csharp
public interface IFutureDefinition<TFuture> : IFutureDefinition, IDefinition
```

#### Type Parameters

`TFuture`<br/>

Implements [IFutureDefinition](../masstransit/ifuturedefinition), [IDefinition](../masstransit/idefinition)

## Properties

### **EndpointDefinition**

Sets the endpoint definition, if available

```csharp
public abstract IEndpointDefinition<TFuture> EndpointDefinition { set; }
```

#### Property Value

[IEndpointDefinition\<TFuture\>](../masstransit/iendpointdefinition-1)<br/>

## Methods

### **Configure(IReceiveEndpointConfigurator, ISagaConfigurator\<FutureState\>, IRegistrationContext)**

Configure the future on the receive endpoint

```csharp
void Configure(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FutureState> sagaConfigurator, IRegistrationContext context)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The receive endpoint configurator for the consumer

`sagaConfigurator` [ISagaConfigurator\<FutureState\>](../masstransit/isagaconfigurator-1)<br/>
The consumer configurator

`context` [IRegistrationContext](../masstransit/iregistrationcontext)<br/>
