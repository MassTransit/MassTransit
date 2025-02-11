---

title: ISagaMessageSpecification<TSaga>

---

# ISagaMessageSpecification\<TSaga\>

Namespace: MassTransit.Configuration

```csharp
public interface ISagaMessageSpecification<TSaga> : IPipeConfigurator<SagaConsumeContext<TSaga>>, ISagaConfigurationObserverConnector, ISpecification
```

#### Type Parameters

`TSaga`<br/>

Implements [IPipeConfigurator\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **MessageType**

```csharp
public abstract Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Methods

### **GetMessageSpecification\<T\>()**

```csharp
ISagaMessageSpecification<TSaga, T> GetMessageSpecification<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISagaMessageSpecification\<TSaga, T\>](../masstransit-configuration/isagamessagespecification-2)<br/>
