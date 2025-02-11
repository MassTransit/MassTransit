---

title: MessageSchedulerContext

---

# MessageSchedulerContext

Namespace: MassTransit

```csharp
public interface MessageSchedulerContext : IMessageScheduler
```

Implements [IMessageScheduler](../masstransit/imessagescheduler)

## Properties

### **SchedulerFactory**

```csharp
public abstract MessageSchedulerFactory SchedulerFactory { get; }
```

#### Property Value

[MessageSchedulerFactory](../masstransit/messageschedulerfactory)<br/>

## Methods

### **ScheduleSend\<T\>(DateTime, T, CancellationToken)**

Send a message

```csharp
Task<ScheduledMessage<T>> ScheduleSend<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(DateTime, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

Send a message

```csharp
Task<ScheduledMessage<T>> ScheduleSend<T>(DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(DateTime, T, IPipe\<SendContext\>, CancellationToken)**

Send a message

```csharp
Task<ScheduledMessage<T>> ScheduleSend<T>(DateTime scheduledTime, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(DateTime, Object, CancellationToken)**

Sends an object as a message, using the type of the message instance.

```csharp
Task<ScheduledMessage> ScheduleSend(DateTime scheduledTime, object message, CancellationToken cancellationToken)
```

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(DateTime, Object, Type, CancellationToken)**

Sends an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
Task<ScheduledMessage> ScheduleSend(DateTime scheduledTime, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the message (use message.GetType() if desired)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(DateTime, Object, IPipe\<SendContext\>, CancellationToken)**

Sends an object as a message.

```csharp
Task<ScheduledMessage> ScheduleSend(DateTime scheduledTime, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(DateTime, Object, Type, IPipe\<SendContext\>, CancellationToken)**

Sends an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
Task<ScheduledMessage> ScheduleSend(DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
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

### **ScheduleSend\<T\>(DateTime, Object, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
Task<ScheduledMessage<T>> ScheduleSend<T>(DateTime scheduledTime, object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(DateTime, Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
Task<ScheduledMessage<T>> ScheduleSend<T>(DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(DateTime, Object, IPipe\<SendContext\>, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
Task<ScheduledMessage<T>> ScheduleSend<T>(DateTime scheduledTime, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker
