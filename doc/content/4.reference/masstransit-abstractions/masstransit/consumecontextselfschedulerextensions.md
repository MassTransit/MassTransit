---

title: ConsumeContextSelfSchedulerExtensions

---

# ConsumeContextSelfSchedulerExtensions

Namespace: MassTransit

```csharp
public static class ConsumeContextSelfSchedulerExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextSelfSchedulerExtensions](../masstransit/consumecontextselfschedulerextensions)

## Methods

### **ScheduleSend\<T\>(ConsumeContext, DateTime, T, CancellationToken)**

Send a message

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(ConsumeContext context, DateTime scheduledTime, T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(ConsumeContext, DateTime, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

Send a message

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(ConsumeContext context, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(ConsumeContext, DateTime, T, IPipe\<SendContext\>, CancellationToken)**

Send a message

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(ConsumeContext context, DateTime scheduledTime, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(ConsumeContext, DateTime, Object, CancellationToken)**

Sends an object as a message, using the type of the message instance.

```csharp
public static Task<ScheduledMessage> ScheduleSend(ConsumeContext context, DateTime scheduledTime, object message, CancellationToken cancellationToken)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(ConsumeContext, DateTime, Object, Type, CancellationToken)**

Sends an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
public static Task<ScheduledMessage> ScheduleSend(ConsumeContext context, DateTime scheduledTime, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

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

### **ScheduleSend(ConsumeContext, DateTime, Object, IPipe\<SendContext\>, CancellationToken)**

Sends an object as a message.

```csharp
public static Task<ScheduledMessage> ScheduleSend(ConsumeContext context, DateTime scheduledTime, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(ConsumeContext, DateTime, Object, Type, IPipe\<SendContext\>, CancellationToken)**

Sends an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
public static Task<ScheduledMessage> ScheduleSend(ConsumeContext context, DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

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

### **ScheduleSend\<T\>(ConsumeContext, DateTime, Object, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(ConsumeContext context, DateTime scheduledTime, object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(ConsumeContext, DateTime, Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(ConsumeContext context, DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(ConsumeContext, DateTime, Object, IPipe\<SendContext\>, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(ConsumeContext context, DateTime scheduledTime, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(ConsumeContext, TimeSpan, T, CancellationToken)**

Send a message

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(ConsumeContext context, TimeSpan delay, T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(ConsumeContext, TimeSpan, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

Send a message

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(ConsumeContext context, TimeSpan delay, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(ConsumeContext, TimeSpan, T, IPipe\<SendContext\>, CancellationToken)**

Send a message

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(ConsumeContext context, TimeSpan delay, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(ConsumeContext, TimeSpan, Object, CancellationToken)**

Sends an object as a message, using the type of the message instance.

```csharp
public static Task<ScheduledMessage> ScheduleSend(ConsumeContext context, TimeSpan delay, object message, CancellationToken cancellationToken)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(ConsumeContext, TimeSpan, Object, Type, CancellationToken)**

Sends an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
public static Task<ScheduledMessage> ScheduleSend(ConsumeContext context, TimeSpan delay, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

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

### **ScheduleSend(ConsumeContext, TimeSpan, Object, IPipe\<SendContext\>, CancellationToken)**

Sends an object as a message.

```csharp
public static Task<ScheduledMessage> ScheduleSend(ConsumeContext context, TimeSpan delay, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message object

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(ConsumeContext, TimeSpan, Object, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(ConsumeContext context, TimeSpan delay, object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend(ConsumeContext, TimeSpan, Object, Type, IPipe\<SendContext\>, CancellationToken)**

Sends an object as a message, using the message type specified. If the object cannot be cast
 to the specified message type, an exception will be thrown.

```csharp
public static Task<ScheduledMessage> ScheduleSend(ConsumeContext context, TimeSpan delay, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

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

### **ScheduleSend\<T\>(ConsumeContext, TimeSpan, Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(ConsumeContext context, TimeSpan delay, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker

### **ScheduleSend\<T\>(ConsumeContext, TimeSpan, Object, IPipe\<SendContext\>, CancellationToken)**

Sends an interface message, initializing the properties of the interface using the anonymous
 object specified

```csharp
public static Task<ScheduledMessage<T>> ScheduleSend<T>(ConsumeContext context, TimeSpan delay, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The interface type to send

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consume context

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time at which the message should be delivered to the queue

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The property values to initialize on the interface

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The task which is completed once the Send is acknowledged by the broker
