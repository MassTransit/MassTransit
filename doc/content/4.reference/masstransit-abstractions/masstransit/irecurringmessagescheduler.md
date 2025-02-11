---

title: IRecurringMessageScheduler

---

# IRecurringMessageScheduler

Namespace: MassTransit

A message scheduler is able to schedule a message for delivery.

```csharp
public interface IRecurringMessageScheduler
```

## Methods

### **ScheduleRecurringSend\<T\>(Uri, RecurringSchedule, T, CancellationToken)**

Send a message

```csharp
Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` T<br/>
The message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend\<T\>(Uri, RecurringSchedule, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

Send a message

```csharp
Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend\<T\>(Uri, RecurringSchedule, T, IPipe\<SendContext\>, CancellationToken)**

Send a message

```csharp
Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend(Uri, RecurringSchedule, Object, CancellationToken)**

Sends an object as a message, using the type of the message instance.

```csharp
Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend(Uri, RecurringSchedule, Object, Type, CancellationToken)**

Sends an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the message (use message.GetType() if desired)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend(Uri, RecurringSchedule, Object, IPipe\<SendContext\>, CancellationToken)**

Sends an object as a message.

```csharp
Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend(Uri, RecurringSchedule, Object, Type, IPipe\<SendContext\>, CancellationToken)**

Sends an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the message (use message.GetType() if desired)

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend\<T\>(Uri, RecurringSchedule, Object, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend\<T\>(Uri, RecurringSchedule, Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringSend\<T\>(Uri, RecurringSchedule, Object, IPipe\<SendContext\>, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringPublish\<T\>(RecurringSchedule, T, CancellationToken)**

Send a message

```csharp
Task<ScheduledRecurringMessage<T>> ScheduleRecurringPublish<T>(RecurringSchedule schedule, T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` T<br/>
The message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleRecurringPublish\<T\>(RecurringSchedule, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

Publish a message

```csharp
Task<ScheduledRecurringMessage<T>> ScheduleRecurringPublish<T>(RecurringSchedule schedule, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **ScheduleRecurringPublish\<T\>(RecurringSchedule, T, IPipe\<SendContext\>, CancellationToken)**

Publish a message

```csharp
Task<ScheduledRecurringMessage<T>> ScheduleRecurringPublish<T>(RecurringSchedule schedule, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **ScheduleRecurringPublish(RecurringSchedule, Object, CancellationToken)**

Publishes an object as a message, using the type of the message instance.

```csharp
Task<ScheduledRecurringMessage> ScheduleRecurringPublish(RecurringSchedule schedule, object message, CancellationToken cancellationToken)
```

#### Parameters

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **ScheduleRecurringPublish(RecurringSchedule, Object, Type, CancellationToken)**

Publishes an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
Task<ScheduledRecurringMessage> ScheduleRecurringPublish(RecurringSchedule schedule, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the message (use message.GetType() if desired)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **ScheduleRecurringPublish(RecurringSchedule, Object, IPipe\<SendContext\>, CancellationToken)**

Publishes an object as a message.

```csharp
Task<ScheduledRecurringMessage> ScheduleRecurringPublish(RecurringSchedule schedule, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **ScheduleRecurringPublish(RecurringSchedule, Object, Type, IPipe\<SendContext\>, CancellationToken)**

Publishes an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
Task<ScheduledRecurringMessage> ScheduleRecurringPublish(RecurringSchedule schedule, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the message (use message.GetType() if desired)

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **ScheduleRecurringPublish\<T\>(RecurringSchedule, Object, CancellationToken)**

Publishes an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
Task<ScheduledRecurringMessage<T>> ScheduleRecurringPublish<T>(RecurringSchedule schedule, object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **ScheduleRecurringPublish\<T\>(RecurringSchedule, Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

Publishes an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
Task<ScheduledRecurringMessage<T>> ScheduleRecurringPublish<T>(RecurringSchedule schedule, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **ScheduleRecurringPublish\<T\>(RecurringSchedule, Object, IPipe\<SendContext\>, CancellationToken)**

Publishes an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
Task<ScheduledRecurringMessage<T>> ScheduleRecurringPublish<T>(RecurringSchedule schedule, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`schedule` [RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>
The schedule for the message to be delivered

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Publish is acknowledged by the broker

### **CancelScheduledRecurringSend(String, String)**

Cancel a scheduled message by TokenId

```csharp
Task CancelScheduledRecurringSend(string scheduleId, string scheduleGroup)
```

#### Parameters

`scheduleId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The scheduleId from the recurring schedule

`scheduleGroup` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The scheduleGroup from the recurring schedule

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PauseScheduledRecurringSend(String, String)**

Pause a scheduled message by ScheduleId and ScheduleGroup.

```csharp
Task PauseScheduledRecurringSend(string scheduleId, string scheduleGroup)
```

#### Parameters

`scheduleId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The scheduleId from the recurring schedule

`scheduleGroup` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The scheduleGroup from the recurring schedule

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ResumeScheduledRecurringSend(String, String)**

Resume a scheduled message by ScheduleId and ScheduleGroup.

```csharp
Task ResumeScheduledRecurringSend(string scheduleId, string scheduleGroup)
```

#### Parameters

`scheduleId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The scheduleId from the recurring schedule

`scheduleGroup` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The scheduleGroup from the recurring schedule

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
