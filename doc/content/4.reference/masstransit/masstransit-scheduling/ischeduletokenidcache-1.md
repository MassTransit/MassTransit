---

title: IScheduleTokenIdCache<T>

---

# IScheduleTokenIdCache\<T\>

Namespace: MassTransit.Scheduling

```csharp
public interface IScheduleTokenIdCache<T>
```

#### Type Parameters

`T`<br/>

## Methods

### **TryGetTokenId(T, Guid)**

Try to get the tokenId for the scheduler from the message

```csharp
bool TryGetTokenId(T message, out Guid tokenId)
```

#### Parameters

`message` T<br/>

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
