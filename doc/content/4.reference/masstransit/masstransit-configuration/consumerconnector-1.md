---

title: ConsumerConnector<T>

---

# ConsumerConnector\<T\>

Namespace: MassTransit.Configuration

```csharp
public class ConsumerConnector<T> : IConsumerConnector
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerConnector\<T\>](../masstransit-configuration/consumerconnector-1)<br/>
Implements [IConsumerConnector](../masstransit-configuration/iconsumerconnector)

## Properties

### **Connectors**

```csharp
public IEnumerable<IConsumerMessageConnector> Connectors { get; }
```

#### Property Value

[IEnumerable\<IConsumerMessageConnector\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Constructors

### **ConsumerConnector()**

```csharp
public ConsumerConnector()
```
