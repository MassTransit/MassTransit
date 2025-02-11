---

title: PublishRecurringMessageScheduler

---

# PublishRecurringMessageScheduler

Namespace: MassTransit.Scheduling

```csharp
public class PublishRecurringMessageScheduler : IRecurringMessageScheduler
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishRecurringMessageScheduler](../masstransit-scheduling/publishrecurringmessagescheduler)<br/>
Implements [IRecurringMessageScheduler](../../masstransit-abstractions/masstransit/irecurringmessagescheduler)

## Constructors

### **PublishRecurringMessageScheduler(IPublishEndpoint, IBusTopology)**

```csharp
public PublishRecurringMessageScheduler(IPublishEndpoint publishEndpoint, IBusTopology busTopology)
```

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

`busTopology` [IBusTopology](../../masstransit-abstractions/masstransit/ibustopology)<br/>

## Methods

### **ScheduleRecurringSend\<T\>(Uri, RecurringSchedule, T, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringSend\<T\>(Uri, RecurringSchedule, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringSend\<T\>(Uri, RecurringSchedule, T, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` T<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringSend(Uri, RecurringSchedule, Object, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringSend(Uri, RecurringSchedule, Object, Type, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringSend(Uri, RecurringSchedule, Object, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringSend(Uri, RecurringSchedule, Object, Type, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringSend\<T\>(Uri, RecurringSchedule, Object, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringSend\<T\>(Uri, RecurringSchedule, Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringSend\<T\>(Uri, RecurringSchedule, Object, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringPublish\<T\>(RecurringSchedule, T, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage<T>> ScheduleRecurringPublish<T>(RecurringSchedule schedule, T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringPublish\<T\>(RecurringSchedule, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage<T>> ScheduleRecurringPublish<T>(RecurringSchedule schedule, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringPublish\<T\>(RecurringSchedule, T, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage<T>> ScheduleRecurringPublish<T>(RecurringSchedule schedule, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` T<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringPublish(RecurringSchedule, Object, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage> ScheduleRecurringPublish(RecurringSchedule schedule, object message, CancellationToken cancellationToken)
```

#### Parameters

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringPublish(RecurringSchedule, Object, Type, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage> ScheduleRecurringPublish(RecurringSchedule schedule, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringPublish(RecurringSchedule, Object, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage> ScheduleRecurringPublish(RecurringSchedule schedule, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringPublish(RecurringSchedule, Object, Type, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage> ScheduleRecurringPublish(RecurringSchedule schedule, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringPublish\<T\>(RecurringSchedule, Object, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage<T>> ScheduleRecurringPublish<T>(RecurringSchedule schedule, object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringPublish\<T\>(RecurringSchedule, Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage<T>> ScheduleRecurringPublish<T>(RecurringSchedule schedule, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringPublish\<T\>(RecurringSchedule, Object, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledRecurringMessage<T>> ScheduleRecurringPublish<T>(RecurringSchedule schedule, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CancelScheduledRecurringSend(String, String)**

```csharp
public Task CancelScheduledRecurringSend(string scheduleId, string scheduleGroup)
```

#### Parameters

`scheduleId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`scheduleGroup` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PauseScheduledRecurringSend(String, String)**

```csharp
public Task PauseScheduledRecurringSend(string scheduleId, string scheduleGroup)
```

#### Parameters

`scheduleId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`scheduleGroup` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ResumeScheduledRecurringSend(String, String)**

```csharp
public Task ResumeScheduledRecurringSend(string scheduleId, string scheduleGroup)
```

#### Parameters

`scheduleId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`scheduleGroup` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
