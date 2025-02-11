---

title: InMemoryBusTopology

---

# InMemoryBusTopology

Namespace: MassTransit.InMemoryTransport

```csharp
public class InMemoryBusTopology : BusTopology, IBusTopology, IInMemoryBusTopology
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusTopology](../masstransit-transports/bustopology) → [InMemoryBusTopology](../masstransit-inmemorytransport/inmemorybustopology)<br/>
Implements [IBusTopology](../../masstransit-abstractions/masstransit/ibustopology), [IInMemoryBusTopology](../masstransit/iinmemorybustopology)

## Properties

### **PublishTopology**

```csharp
public IPublishTopology PublishTopology { get; }
```

#### Property Value

[IPublishTopology](../../masstransit-abstractions/masstransit/ipublishtopology)<br/>

### **SendTopology**

```csharp
public ISendTopology SendTopology { get; }
```

#### Property Value

[ISendTopology](../../masstransit-abstractions/masstransit/isendtopology)<br/>

## Constructors

### **InMemoryBusTopology(IInMemoryHostConfiguration, IInMemoryTopologyConfiguration)**

```csharp
public InMemoryBusTopology(IInMemoryHostConfiguration hostConfiguration, IInMemoryTopologyConfiguration configuration)
```

#### Parameters

`hostConfiguration` [IInMemoryHostConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryhostconfiguration)<br/>

`configuration` [IInMemoryTopologyConfiguration](../masstransit-inmemorytransport-configuration/iinmemorytopologyconfiguration)<br/>

## Methods

### **Publish\<T\>()**

```csharp
public IInMemoryMessagePublishTopology<T> Publish<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IInMemoryMessagePublishTopology\<T\>](../masstransit/iinmemorymessagepublishtopology-1)<br/>
