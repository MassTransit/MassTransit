---

title: IMessageCorrelationId<T>

---

# IMessageCorrelationId\<T\>

Namespace: MassTransit

```csharp
public interface IMessageCorrelationId<T>
```

#### Type Parameters

`T`<br/>

## Methods

### **TryGetCorrelationId(T, Guid)**

Get the CorrelationId from the message, if available

```csharp
bool TryGetCorrelationId(T message, out Guid correlationId)
```

#### Parameters

`message` T<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
