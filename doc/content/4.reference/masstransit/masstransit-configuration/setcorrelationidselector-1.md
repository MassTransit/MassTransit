---

title: SetCorrelationIdSelector<T>

---

# SetCorrelationIdSelector\<T\>

Namespace: MassTransit.Configuration

```csharp
public class SetCorrelationIdSelector<T> : ICorrelationIdSelector<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SetCorrelationIdSelector\<T\>](../masstransit-configuration/setcorrelationidselector-1)<br/>
Implements [ICorrelationIdSelector\<T\>](../masstransit-configuration/icorrelationidselector-1)

## Constructors

### **SetCorrelationIdSelector(IMessageCorrelationId\<T\>)**

```csharp
public SetCorrelationIdSelector(IMessageCorrelationId<T> messageCorrelationId)
```

#### Parameters

`messageCorrelationId` [IMessageCorrelationId\<T\>](../masstransit/imessagecorrelationid-1)<br/>

## Methods

### **TryGetSetCorrelationId(IMessageCorrelationId\<T\>)**

```csharp
public bool TryGetSetCorrelationId(out IMessageCorrelationId<T> messageCorrelationId)
```

#### Parameters

`messageCorrelationId` [IMessageCorrelationId\<T\>](../masstransit/imessagecorrelationid-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
