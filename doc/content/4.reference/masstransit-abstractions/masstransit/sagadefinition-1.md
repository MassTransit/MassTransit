---

title: SagaDefinition<TSaga>

---

# SagaDefinition\<TSaga\>

Namespace: MassTransit

A saga definition defines the configuration for a saga, which can be used by the automatic registration code to
 configure the consumer on a receive endpoint.

```csharp
public class SagaDefinition<TSaga> : ISagaDefinition<TSaga>, ISagaDefinition, IDefinition
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaDefinition\<TSaga\>](../masstransit/sagadefinition-1)<br/>
Implements [ISagaDefinition\<TSaga\>](../masstransit/isagadefinition-1), [ISagaDefinition](../masstransit/isagadefinition), [IDefinition](../masstransit/idefinition)

## Properties

### **EndpointDefinition**

```csharp
public IEndpointDefinition<TSaga> EndpointDefinition { get; set; }
```

#### Property Value

[IEndpointDefinition\<TSaga\>](../masstransit/iendpointdefinition-1)<br/>

### **ConcurrentMessageLimit**

Set the concurrent message limit for the saga, which limits how many saga instances are able to concurrently
 consume messages.

```csharp
public Nullable<int> ConcurrentMessageLimit { get; protected set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **ConfigureSaga(IReceiveEndpointConfigurator, ISagaConfigurator\<TSaga\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Called when configuring the saga on the endpoint. Configuration only applies to this saga, and does not apply to
 the endpoint.

```csharp
protected void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<TSaga> sagaConfigurator)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The receive endpoint configurator for the consumer

`sagaConfigurator` [ISagaConfigurator\<TSaga\>](../masstransit/isagaconfigurator-1)<br/>
The saga configurator

### **ConfigureSaga(IReceiveEndpointConfigurator, ISagaConfigurator\<TSaga\>, IRegistrationContext)**

Called when configuring the saga on the endpoint. Configuration only applies to this saga, and does not apply to
 the endpoint.

```csharp
protected void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<TSaga> sagaConfigurator, IRegistrationContext context)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The receive endpoint configurator for the consumer

`sagaConfigurator` [ISagaConfigurator\<TSaga\>](../masstransit/isagaconfigurator-1)<br/>
The saga configurator

`context` [IRegistrationContext](../masstransit/iregistrationcontext)<br/>

### **Endpoint(Action\<IEndpointRegistrationConfigurator\>)**

Configure the saga endpoint

```csharp
protected void Endpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
