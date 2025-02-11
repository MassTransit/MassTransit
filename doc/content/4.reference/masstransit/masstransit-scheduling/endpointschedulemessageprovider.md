---

title: EndpointScheduleMessageProvider

---

# EndpointScheduleMessageProvider

Namespace: MassTransit.Scheduling

```csharp
public class EndpointScheduleMessageProvider : BaseScheduleMessageProvider, IScheduleMessageProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseScheduleMessageProvider](../masstransit-scheduling/baseschedulemessageprovider) → [EndpointScheduleMessageProvider](../masstransit-scheduling/endpointschedulemessageprovider)<br/>
Implements [IScheduleMessageProvider](../../masstransit-abstractions/masstransit/ischedulemessageprovider)

## Constructors

### **EndpointScheduleMessageProvider(Func\<Task\<ISendEndpoint\>\>)**

```csharp
public EndpointScheduleMessageProvider(Func<Task<ISendEndpoint>> schedulerEndpoint)
```

#### Parameters

`schedulerEndpoint` [Func\<Task\<ISendEndpoint\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

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
