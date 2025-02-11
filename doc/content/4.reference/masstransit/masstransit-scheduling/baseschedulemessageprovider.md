---

title: BaseScheduleMessageProvider

---

# BaseScheduleMessageProvider

Namespace: MassTransit.Scheduling

```csharp
public abstract class BaseScheduleMessageProvider : IScheduleMessageProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseScheduleMessageProvider](../masstransit-scheduling/baseschedulemessageprovider)<br/>
Implements [IScheduleMessageProvider](../../masstransit-abstractions/masstransit/ischedulemessageprovider)

## Methods

### **ScheduleSend\<T\>(Uri, DateTime, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CancelScheduledSend(Guid, CancellationToken)**

```csharp
public Task CancelScheduledSend(Guid tokenId, CancellationToken cancellationToken)
```

#### Parameters

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CancelScheduledSend(Uri, Guid, CancellationToken)**

```csharp
public Task CancelScheduledSend(Uri destinationAddress, Guid tokenId, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ScheduleSend(ScheduleMessage, IPipe\<SendContext\<ScheduleMessage\>\>, CancellationToken)**

```csharp
protected abstract Task ScheduleSend(ScheduleMessage message, IPipe<SendContext<ScheduleMessage>> pipe, CancellationToken cancellationToken)
```

#### Parameters

`message` [ScheduleMessage](../../masstransit-abstractions/masstransit-scheduling/schedulemessage)<br/>

`pipe` [IPipe\<SendContext\<ScheduleMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CancelScheduledSend(Guid, Uri, CancellationToken)**

```csharp
protected abstract Task CancelScheduledSend(Guid tokenId, Uri destinationAddress, CancellationToken cancellationToken)
```

#### Parameters

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`destinationAddress` Uri<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
