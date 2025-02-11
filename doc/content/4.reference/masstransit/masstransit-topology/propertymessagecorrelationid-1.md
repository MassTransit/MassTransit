---

title: PropertyMessageCorrelationId<T>

---

# PropertyMessageCorrelationId\<T\>

Namespace: MassTransit.Topology

```csharp
public class PropertyMessageCorrelationId<T> : IMessageCorrelationId<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PropertyMessageCorrelationId\<T\>](../masstransit-topology/propertymessagecorrelationid-1)<br/>
Implements [IMessageCorrelationId\<T\>](../masstransit/imessagecorrelationid-1)

## Constructors

### **PropertyMessageCorrelationId(IReadProperty\<T, Guid\>)**

```csharp
public PropertyMessageCorrelationId(IReadProperty<T, Guid> property)
```

#### Parameters

`property` [IReadProperty\<T, Guid\>](../masstransit-internals/ireadproperty-2)<br/>

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
