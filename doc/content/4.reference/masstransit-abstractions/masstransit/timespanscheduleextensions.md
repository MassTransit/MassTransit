---

title: TimeSpanScheduleExtensions

---

# TimeSpanScheduleExtensions

Namespace: MassTransit

```csharp
public static class TimeSpanScheduleExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TimeSpanScheduleExtensions](../masstransit/timespanscheduleextensions)

## Methods

### **ScheduleSend\<T\>(IMessageScheduler, Uri, TimeSpan, T, CancellationToken)**

Send a message

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(IMessageScheduler, Uri, TimeSpan, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

Send a message

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(IMessageScheduler, Uri, TimeSpan, T, Action\<SendContext\<T\>\>, CancellationToken)**

Send a message

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, T message, Action<SendContext<T>> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(IMessageScheduler, Uri, TimeSpan, T, Func\<SendContext\<T\>, Task\>, CancellationToken)**

Send a message

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, T message, Func<SendContext<T>, Task> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`callback` [Func\<SendContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(IMessageScheduler, Uri, TimeSpan, T, IPipe\<SendContext\>, CancellationToken)**

Send a message

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(IMessageScheduler, Uri, TimeSpan, T, Action\<SendContext\>, CancellationToken)**

Send a message

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, T message, Action<SendContext> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`callback` [Action\<SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
The cancellation token

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(IMessageScheduler, Uri, TimeSpan, T, Func\<SendContext, Task\>, CancellationToken)**

Send a message

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, T message, Func<SendContext, Task> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`callback` [Func\<SendContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
The cancellation token

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(IMessageScheduler, Uri, TimeSpan, Object, CancellationToken)**

Sends an object as a message, using the type of the message instance.

```csharp
public static Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object message, CancellationToken cancellationToken)
```

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(IMessageScheduler, Uri, TimeSpan, Object, Type, CancellationToken)**

Sends an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
public static Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the message (use message.GetType() if desired)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(IMessageScheduler, Uri, TimeSpan, Object, IPipe\<SendContext\>, CancellationToken)**

Sends an object as a message.

```csharp
public static Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(IMessageScheduler, Uri, TimeSpan, Object, Action\<SendContext\>, CancellationToken)**

Sends an object as a message.

```csharp
public static Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object message, Action<SendContext> callback, CancellationToken cancellationToken)
```

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`callback` [Action\<SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
The token used to cancel the operation

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(IMessageScheduler, Uri, TimeSpan, Object, Func\<SendContext, Task\>, CancellationToken)**

Sends an object as a message.

```csharp
public static Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object message, Func<SendContext, Task> callback, CancellationToken cancellationToken)
```

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`callback` [Func\<SendContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback for the send context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
The token used to cancel the operation

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(IMessageScheduler, Uri, TimeSpan, Object, Type, IPipe\<SendContext\>, CancellationToken)**

Sends an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
public static Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the message (use message.GetType() if desired)

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(IMessageScheduler, Uri, TimeSpan, Object, Type, Action\<SendContext\>, CancellationToken)**

Sends an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
public static Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object message, Type messageType, Action<SendContext> callback, CancellationToken cancellationToken)
```

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the message (use message.GetType() if desired)

`callback` [Action\<SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The send callback

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(IMessageScheduler, Uri, TimeSpan, Object, Type, Func\<SendContext, Task\>, CancellationToken)**

Sends an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
public static Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object message, Type messageType, Func<SendContext, Task> callback, CancellationToken cancellationToken)
```

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the message (use message.GetType() if desired)

`callback` [Func\<SendContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The send callback

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(IMessageScheduler, Uri, TimeSpan, Object, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(IMessageScheduler, Uri, TimeSpan, Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(IMessageScheduler, Uri, TimeSpan, Object, Action\<SendContext\<T\>\>, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object values, Action<SendContext<T>> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The send callback

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(IMessageScheduler, Uri, TimeSpan, Object, Func\<SendContext\<T\>, Task\>, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object values, Func<SendContext<T>, Task> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`callback` [Func\<SendContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The send callback

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(IMessageScheduler, Uri, TimeSpan, Object, IPipe\<SendContext\>, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(IMessageScheduler, Uri, TimeSpan, Object, Action\<SendContext\>, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object values, Action<SendContext> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`callback` [Action\<SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The send callback

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(IMessageScheduler, Uri, TimeSpan, Object, Func\<SendContext, Task\>, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(IMessageScheduler scheduler, Uri destinationAddress, TimeSpan delay, object values, Func<SendContext, Task> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`destinationAddress` Uri<br/>
The destination address where the schedule message should be sent

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`callback` [Func\<SendContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The send callback

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker
