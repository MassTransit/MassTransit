---

title: ISagaDefinition<TSaga>

---

# ISagaDefinition\<TSaga\>

Namespace: MassTransit

```csharp
public interface ISagaDefinition<TSaga> : ISagaDefinition, IDefinition
```

#### Type Parameters

`TSaga`<br/>

Implements [ISagaDefinition](../masstransit/isagadefinition), [IDefinition](../masstransit/idefinition)

## Properties

### **EndpointDefinition**

Sets the endpoint definition, if available

```csharp
public abstract IEndpointDefinition<TSaga> EndpointDefinition { set; }
```

#### Property Value

[IEndpointDefinition\<TSaga\>](../masstransit/iendpointdefinition-1)<br/>

## Methods

### **Configure(IReceiveEndpointConfigurator, ISagaConfigurator\<TSaga\>, IRegistrationContext)**

Configure the consumer on the receive endpoint

```csharp
void Configure(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<TSaga> sagaConfigurator, IRegistrationContext context)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The receive endpoint configurator for the consumer

`sagaConfigurator` [ISagaConfigurator\<TSaga\>](../masstransit/isagaconfigurator-1)<br/>
The consumer configurator

`context` [IRegistrationContext](../masstransit/iregistrationcontext)<br/>
