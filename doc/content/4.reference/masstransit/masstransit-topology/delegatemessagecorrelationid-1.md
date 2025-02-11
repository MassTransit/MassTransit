---

title: DelegateMessageCorrelationId<T>

---

# DelegateMessageCorrelationId\<T\>

Namespace: MassTransit.Topology

```csharp
public class DelegateMessageCorrelationId<T> : IMessageCorrelationId<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelegateMessageCorrelationId\<T\>](../masstransit-topology/delegatemessagecorrelationid-1)<br/>
Implements [IMessageCorrelationId\<T\>](../masstransit/imessagecorrelationid-1)

## Constructors

### **DelegateMessageCorrelationId(Func\<T, Guid\>)**

```csharp
public DelegateMessageCorrelationId(Func<T, Guid> getCorrelationId)
```

#### Parameters

`getCorrelationId` [Func\<T, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
