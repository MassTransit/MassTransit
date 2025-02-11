---

title: PublishScheduleMessageProvider

---

# PublishScheduleMessageProvider

Namespace: MassTransit.Scheduling

```csharp
public class PublishScheduleMessageProvider : BaseScheduleMessageProvider, IScheduleMessageProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseScheduleMessageProvider](../masstransit-scheduling/baseschedulemessageprovider) → [PublishScheduleMessageProvider](../masstransit-scheduling/publishschedulemessageprovider)<br/>
Implements [IScheduleMessageProvider](../../masstransit-abstractions/masstransit/ischedulemessageprovider)

## Constructors

### **PublishScheduleMessageProvider(IPublishEndpoint)**

```csharp
public PublishScheduleMessageProvider(IPublishEndpoint publishEndpoint)
```

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

## Methods

### **ScheduleSend(ScheduleMessage, IPipe\<SendContext\<ScheduleMessage\>\>, CancellationToken)**

```csharp
protected Task ScheduleSend(ScheduleMessage message, IPipe<SendContext<ScheduleMessage>> pipe, CancellationToken cancellationToken)
```

#### Parameters

`message` [ScheduleMessage](../../masstransit-abstractions/masstransit-scheduling/schedulemessage)<br/>

`pipe` [IPipe\<SendContext\<ScheduleMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CancelScheduledSend(Guid, Uri, CancellationToken)**

```csharp
protected Task CancelScheduledSend(Guid tokenId, Uri destinationAddress, CancellationToken cancellationToken)
```

#### Parameters

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`destinationAddress` Uri<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
