---

title: MessageScheduler

---

# MessageScheduler

Namespace: MassTransit.Scheduling

```csharp
public class MessageScheduler : IMessageScheduler
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageScheduler](../masstransit-scheduling/messagescheduler)<br/>
Implements [IMessageScheduler](../../masstransit-abstractions/masstransit/imessagescheduler)

## Constructors

### **MessageScheduler(IScheduleMessageProvider, IBusTopology)**

```csharp
public MessageScheduler(IScheduleMessageProvider provider, IBusTopology busTopology)
```

#### Parameters

`provider` [IScheduleMessageProvider](../../masstransit-abstractions/masstransit/ischedulemessageprovider)<br/>

`busTopology` [IBusTopology](../../masstransit-abstractions/masstransit/ibustopology)<br/>

## Methods

### **ScheduleSend\<T\>(Uri, DateTime, T, CancellationToken)**

```csharp
public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

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

### **ScheduleSend\<T\>(Uri, DateTime, T, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` T<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleSend(Uri, DateTime, Object, CancellationToken)**

```csharp
public Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleSend(Uri, DateTime, Object, Type, CancellationToken)**

```csharp
public Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleSend(Uri, DateTime, Object, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleSend(Uri, DateTime, Object, Type, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleSend\<T\>(Uri, DateTime, Object, CancellationToken)**

```csharp
public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleSend\<T\>(Uri, DateTime, Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleSend\<T\>(Uri, DateTime, Object, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

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

### **SchedulePublish\<T\>(DateTime, T, CancellationToken)**

```csharp
public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SchedulePublish\<T\>(DateTime, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SchedulePublish\<T\>(DateTime, T, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` T<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SchedulePublish(DateTime, Object, CancellationToken)**

```csharp
public Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, CancellationToken cancellationToken)
```

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SchedulePublish(DateTime, Object, Type, CancellationToken)**

```csharp
public Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SchedulePublish(DateTime, Object, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SchedulePublish(DateTime, Object, Type, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SchedulePublish\<T\>(DateTime, Object, CancellationToken)**

```csharp
public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SchedulePublish\<T\>(DateTime, Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SchedulePublish\<T\>(DateTime, Object, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CancelScheduledPublish\<T\>(Guid, CancellationToken)**

```csharp
public Task CancelScheduledPublish<T>(Guid tokenId, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CancelScheduledPublish(Type, Guid, CancellationToken)**

```csharp
public Task CancelScheduledPublish(Type messageType, Guid tokenId, CancellationToken cancellationToken)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
