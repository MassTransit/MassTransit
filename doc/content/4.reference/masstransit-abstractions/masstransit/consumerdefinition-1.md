---

title: ConsumerDefinition<TConsumer>

---

# ConsumerDefinition\<TConsumer\>

Namespace: MassTransit

A consumer definition defines the configuration for a consumer, which can be used by the automatic registration code to
 configure the consumer on a receive endpoint.

```csharp
public class ConsumerDefinition<TConsumer> : IConsumerDefinition<TConsumer>, IConsumerDefinition, IDefinition
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerDefinition\<TConsumer\>](../masstransit/consumerdefinition-1)<br/>
Implements [IConsumerDefinition\<TConsumer\>](../masstransit/iconsumerdefinition-1), [IConsumerDefinition](../masstransit/iconsumerdefinition), [IDefinition](../masstransit/idefinition)

## Properties

### **EndpointDefinition**

```csharp
public IEndpointDefinition<TConsumer> EndpointDefinition { get; set; }
```

#### Property Value

[IEndpointDefinition\<TConsumer\>](../masstransit/iendpointdefinition-1)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; protected set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **Endpoint(Action\<IEndpointRegistrationConfigurator\>)**

Configure the consumer endpoint

```csharp
protected void Endpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureConsumer(IReceiveEndpointConfigurator, IConsumerConfigurator\<TConsumer\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Called when the consumer is being configured on the endpoint. Configuration only applies to this consumer, and does not apply to
 the endpoint.

```csharp
protected void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TConsumer> consumerConfigurator)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The receive endpoint configurator for the consumer

`consumerConfigurator` [IConsumerConfigurator\<TConsumer\>](../masstransit/iconsumerconfigurator-1)<br/>
The consumer configurator

### **ConfigureConsumer(IReceiveEndpointConfigurator, IConsumerConfigurator\<TConsumer\>, IRegistrationContext)**

Called when the consumer is being configured on the endpoint. Configuration only applies to this consumer, and does not apply to
 the endpoint.

```csharp
protected void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TConsumer> consumerConfigurator, IRegistrationContext context)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The receive endpoint configurator for the consumer

`consumerConfigurator` [IConsumerConfigurator\<TConsumer\>](../masstransit/iconsumerconfigurator-1)<br/>
The consumer configurator

`context` [IRegistrationContext](../masstransit/iregistrationcontext)<br/>
