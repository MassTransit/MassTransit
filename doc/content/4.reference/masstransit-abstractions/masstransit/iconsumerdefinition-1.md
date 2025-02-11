---

title: IConsumerDefinition<TConsumer>

---

# IConsumerDefinition\<TConsumer\>

Namespace: MassTransit

```csharp
public interface IConsumerDefinition<TConsumer> : IConsumerDefinition, IDefinition
```

#### Type Parameters

`TConsumer`<br/>

Implements [IConsumerDefinition](../masstransit/iconsumerdefinition), [IDefinition](../masstransit/idefinition)

## Properties

### **EndpointDefinition**

Sets the endpoint definition, if available

```csharp
public abstract IEndpointDefinition<TConsumer> EndpointDefinition { set; }
```

#### Property Value

[IEndpointDefinition\<TConsumer\>](../masstransit/iendpointdefinition-1)<br/>

## Methods

### **Configure(IReceiveEndpointConfigurator, IConsumerConfigurator\<TConsumer\>, IRegistrationContext)**

Configure the consumer on the receive endpoint

```csharp
void Configure(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TConsumer> consumerConfigurator, IRegistrationContext context)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The receive endpoint configurator for the consumer

`consumerConfigurator` [IConsumerConfigurator\<TConsumer\>](../masstransit/iconsumerconfigurator-1)<br/>
The consumer configurator

`context` [IRegistrationContext](../masstransit/iregistrationcontext)<br/>
