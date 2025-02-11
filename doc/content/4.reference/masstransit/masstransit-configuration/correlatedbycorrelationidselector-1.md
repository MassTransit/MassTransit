---

title: CorrelatedByCorrelationIdSelector<T>

---

# CorrelatedByCorrelationIdSelector\<T\>

Namespace: MassTransit.Configuration

```csharp
public class CorrelatedByCorrelationIdSelector<T> : ICorrelationIdSelector<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CorrelatedByCorrelationIdSelector\<T\>](../masstransit-configuration/correlatedbycorrelationidselector-1)<br/>
Implements [ICorrelationIdSelector\<T\>](../masstransit-configuration/icorrelationidselector-1)

## Constructors

### **CorrelatedByCorrelationIdSelector()**

```csharp
public CorrelatedByCorrelationIdSelector()
```

## Methods

### **TryGetSetCorrelationId(IMessageCorrelationId\<T\>)**

```csharp
public bool TryGetSetCorrelationId(out IMessageCorrelationId<T> messageCorrelationId)
```

#### Parameters

`messageCorrelationId` [IMessageCorrelationId\<T\>](../masstransit/imessagecorrelationid-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
