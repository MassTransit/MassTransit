---

title: ICorrelationIdSelector<T>

---

# ICorrelationIdSelector\<T\>

Namespace: MassTransit.Configuration

```csharp
public interface ICorrelationIdSelector<T>
```

#### Type Parameters

`T`<br/>

## Methods

### **TryGetSetCorrelationId(IMessageCorrelationId\<T\>)**

```csharp
bool TryGetSetCorrelationId(out IMessageCorrelationId<T> messageCorrelationId)
```

#### Parameters

`messageCorrelationId` [IMessageCorrelationId\<T\>](../masstransit/imessagecorrelationid-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
