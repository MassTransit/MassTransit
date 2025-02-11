---

title: CorrelatedByMessageCorrelationId<T>

---

# CorrelatedByMessageCorrelationId\<T\>

Namespace: MassTransit.Topology

```csharp
public class CorrelatedByMessageCorrelationId<T> : IMessageCorrelationId<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CorrelatedByMessageCorrelationId\<T\>](../masstransit-topology/correlatedbymessagecorrelationid-1)<br/>
Implements [IMessageCorrelationId\<T\>](../masstransit/imessagecorrelationid-1)

## Constructors

### **CorrelatedByMessageCorrelationId()**

```csharp
public CorrelatedByMessageCorrelationId()
```

## Methods

### **TryGetCorrelationId(T, Guid)**

```csharp
public bool TryGetCorrelationId(T message, out Guid correlationId)
```

#### Parameters

`message` T<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
