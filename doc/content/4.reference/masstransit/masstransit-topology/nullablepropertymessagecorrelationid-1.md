---

title: NullablePropertyMessageCorrelationId<T>

---

# NullablePropertyMessageCorrelationId\<T\>

Namespace: MassTransit.Topology

```csharp
public class NullablePropertyMessageCorrelationId<T> : IMessageCorrelationId<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NullablePropertyMessageCorrelationId\<T\>](../masstransit-topology/nullablepropertymessagecorrelationid-1)<br/>
Implements [IMessageCorrelationId\<T\>](../masstransit/imessagecorrelationid-1)

## Constructors

### **NullablePropertyMessageCorrelationId(IReadProperty\<T, Nullable\<Guid\>\>)**

```csharp
public NullablePropertyMessageCorrelationId(IReadProperty<T, Nullable<Guid>> property)
```

#### Parameters

`property` [IReadProperty\<T, Nullable\<Guid\>\>](../masstransit-internals/ireadproperty-2)<br/>

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
