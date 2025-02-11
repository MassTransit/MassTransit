---

title: PropertyCorrelationIdSelector<T>

---

# PropertyCorrelationIdSelector\<T\>

Namespace: MassTransit.Configuration

```csharp
public class PropertyCorrelationIdSelector<T> : ICorrelationIdSelector<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PropertyCorrelationIdSelector\<T\>](../masstransit-configuration/propertycorrelationidselector-1)<br/>
Implements [ICorrelationIdSelector\<T\>](../masstransit-configuration/icorrelationidselector-1)

## Constructors

### **PropertyCorrelationIdSelector(String)**

```csharp
public PropertyCorrelationIdSelector(string propertyName)
```

#### Parameters

`propertyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **TryGetSetCorrelationId(IMessageCorrelationId\<T\>)**

```csharp
public bool TryGetSetCorrelationId(out IMessageCorrelationId<T> messageCorrelationId)
```

#### Parameters

`messageCorrelationId` [IMessageCorrelationId\<T\>](../masstransit/imessagecorrelationid-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
