---

title: IScheduleMessageProvider

---

# IScheduleMessageProvider

Namespace: MassTransit

```csharp
public interface IScheduleMessageProvider
```

## Methods

### **ScheduleSend\<T\>(Uri, DateTime, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

Schedule a message to be sent

```csharp
Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CancelScheduledSend(Guid, CancellationToken)**

Cancel a scheduled message by TokenId

```csharp
Task CancelScheduledSend(Guid tokenId, CancellationToken cancellationToken)
```

#### Parameters

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
The tokenId of the scheduled message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CancelScheduledSend(Uri, Guid, CancellationToken)**

Cancel a scheduled message by TokenId

```csharp
Task CancelScheduledSend(Uri destinationAddress, Guid tokenId, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>
The destination address of the scheduled message

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
The tokenId of the scheduled message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
