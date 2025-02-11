---

title: PublishEndpointRecurringSchedulerExtensions

---

# PublishEndpointRecurringSchedulerExtensions

Namespace: MassTransit

```csharp
public static class PublishEndpointRecurringSchedulerExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishEndpointRecurringSchedulerExtensions](../masstransit/publishendpointrecurringschedulerextensions)

## Methods

### **ScheduleRecurringSend\<T\>(IPublishEndpoint, Uri, RecurringSchedule, T, CancellationToken)**

Schedule a message for recurring delivery using the specified schedule

```csharp
public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(IPublishEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule, T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The message scheduler endpoint

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` T<br/>
The message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend\<T\>(IPublishEndpoint, Uri, RecurringSchedule, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

Schedule a message for recurring delivery using the specified schedule

```csharp
public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(IPublishEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The message scheduler endpoint

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend\<T\>(IPublishEndpoint, Uri, RecurringSchedule, T, IPipe\<SendContext\>, CancellationToken)**

Schedule a message for recurring delivery using the specified schedule

```csharp
public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(IPublishEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The message scheduler endpoint

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend(IPublishEndpoint, Uri, RecurringSchedule, Object, CancellationToken)**

Schedule a message for recurring delivery using the specified schedule

```csharp
public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(IPublishEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule, object message, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The message scheduler endpoint

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend(IPublishEndpoint, Uri, RecurringSchedule, Object, Type, CancellationToken)**

Schedule a message for recurring delivery using the specified schedule

```csharp
public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(IPublishEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The message scheduler endpoint

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the message (use message.GetType() if desired)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend(IPublishEndpoint, Uri, RecurringSchedule, Object, IPipe\<SendContext\>, CancellationToken)**

Schedule a message for recurring delivery using the specified schedule

```csharp
public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(IPublishEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The message scheduler endpoint

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend(IPublishEndpoint, Uri, RecurringSchedule, Object, Type, IPipe\<SendContext\>, CancellationToken)**

Schedule a message for recurring delivery using the specified schedule

```csharp
public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(IPublishEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The message scheduler endpoint

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the message (use message.GetType() if desired)

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend\<T\>(IPublishEndpoint, Uri, RecurringSchedule, Object, CancellationToken)**

Schedule a message for recurring delivery using the specified schedule

```csharp
public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(IPublishEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule, object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The message scheduler endpoint

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend\<T\>(IPublishEndpoint, Uri, RecurringSchedule, Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

Schedule a message for recurring delivery using the specified schedule

```csharp
public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(IPublishEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The message scheduler endpoint

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend\<T\>(IPublishEndpoint, Uri, RecurringSchedule, Object, IPipe\<SendContext\>, CancellationToken)**

Schedule a message for recurring delivery using the specified schedule

```csharp
public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(IPublishEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The message scheduler endpoint

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **CancelScheduledRecurringSend\<T\>(IPublishEndpoint, ScheduledRecurringMessage\<T\>)**

Cancel a scheduled message using the scheduled message instance

```csharp
public static Task CancelScheduledRecurringSend<T>(IPublishEndpoint endpoint, ScheduledRecurringMessage<T> message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The endpoint of the scheduling service

`message` [ScheduledRecurringMessage\<T\>](../../masstransit-abstractions/masstransit/scheduledrecurringmessage-1)<br/>
The schedule message reference

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CancelScheduledRecurringSend(IPublishEndpoint, String, String)**

Cancel a scheduled message using the scheduleId and scheduleGroup that was returned when the message was scheduled.

```csharp
public static Task CancelScheduledRecurringSend(IPublishEndpoint endpoint, string scheduleId, string scheduleGroup)
```

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The endpoint of the scheduling service

`scheduleId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The scheduleId from the recurring schedule

`scheduleGroup` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The scheduleGroup from the recurring schedule

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PauseScheduledRecurringSend\<T\>(IPublishEndpoint, ScheduledRecurringMessage\<T\>)**

Pause a scheduled message using the scheduled message instance

```csharp
public static Task PauseScheduledRecurringSend<T>(IPublishEndpoint endpoint, ScheduledRecurringMessage<T> message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The endpoint of the scheduling service

`message` [ScheduledRecurringMessage\<T\>](../../masstransit-abstractions/masstransit/scheduledrecurringmessage-1)<br/>
The schedule message reference

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PauseScheduledRecurringSend(IPublishEndpoint, String, String)**

Pause a scheduled message using the scheduleId and scheduleGroup that was returned when the message was scheduled.

```csharp
public static Task PauseScheduledRecurringSend(IPublishEndpoint endpoint, string scheduleId, string scheduleGroup)
```

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The endpoint of the scheduling service

`scheduleId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The scheduleId from the recurring schedule

`scheduleGroup` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The scheduleGroup from the recurring schedule

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ResumeScheduledRecurringSend\<T\>(IPublishEndpoint, ScheduledRecurringMessage\<T\>)**

Resume a scheduled message using the scheduled message instance

```csharp
public static Task ResumeScheduledRecurringSend<T>(IPublishEndpoint endpoint, ScheduledRecurringMessage<T> message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The endpoint of the scheduling service

`message` [ScheduledRecurringMessage\<T\>](../../masstransit-abstractions/masstransit/scheduledrecurringmessage-1)<br/>
The schedule message reference

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ResumeScheduledRecurringSend(IPublishEndpoint, String, String)**

Resume a scheduled message using the scheduleId and scheduleGroup that was returned when the message was scheduled.

```csharp
public static Task ResumeScheduledRecurringSend(IPublishEndpoint endpoint, string scheduleId, string scheduleGroup)
```

#### Parameters

`endpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
The endpoint of the scheduling service

`scheduleId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The scheduleId from the recurring schedule

`scheduleGroup` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The scheduleGroup from the recurring schedule

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
