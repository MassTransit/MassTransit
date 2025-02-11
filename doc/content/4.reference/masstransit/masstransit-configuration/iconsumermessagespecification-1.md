---

title: IConsumerMessageSpecification<TConsumer>

---

# IConsumerMessageSpecification\<TConsumer\>

Namespace: MassTransit.Configuration

```csharp
public interface IConsumerMessageSpecification<TConsumer> : IPipeConfigurator<ConsumerConsumeContext<TConsumer>>, IConsumerConfigurationObserverConnector, ISpecification
```

#### Type Parameters

`TConsumer`<br/>

Implements [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **MessageType**

```csharp
public abstract Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Methods

### **TryGetMessageSpecification\<TC, T\>(IConsumerMessageSpecification\<TC, T\>)**

```csharp
bool TryGetMessageSpecification<TC, T>(out IConsumerMessageSpecification<TC, T> specification)
```

#### Type Parameters

`TC`<br/>

`T`<br/>

#### Parameters

`specification` [IConsumerMessageSpecification\<TC, T\>](../masstransit-configuration/iconsumermessagespecification-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
