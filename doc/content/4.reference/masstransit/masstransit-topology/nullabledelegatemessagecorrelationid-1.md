---

title: NullableDelegateMessageCorrelationId<T>

---

# NullableDelegateMessageCorrelationId\<T\>

Namespace: MassTransit.Topology

```csharp
public class NullableDelegateMessageCorrelationId<T> : IMessageCorrelationId<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NullableDelegateMessageCorrelationId\<T\>](../masstransit-topology/nullabledelegatemessagecorrelationid-1)<br/>
Implements [IMessageCorrelationId\<T\>](../masstransit/imessagecorrelationid-1)

## Constructors

### **NullableDelegateMessageCorrelationId(Func\<T, Nullable\<Guid\>\>)**

```csharp
public NullableDelegateMessageCorrelationId(Func<T, Nullable<Guid>> getCorrelationId)
```

#### Parameters

`getCorrelationId` [Func\<T, Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
