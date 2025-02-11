---

title: SagaConnector<TSaga>

---

# SagaConnector\<TSaga\>

Namespace: MassTransit.Configuration

```csharp
public class SagaConnector<TSaga> : ISagaConnector
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaConnector\<TSaga\>](../masstransit-configuration/sagaconnector-1)<br/>
Implements [ISagaConnector](../masstransit-configuration/isagaconnector)

## Properties

### **Connectors**

```csharp
public IEnumerable<ISagaMessageConnector> Connectors { get; }
```

#### Property Value

[IEnumerable\<ISagaMessageConnector\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Constructors

### **SagaConnector()**

```csharp
public SagaConnector()
```
