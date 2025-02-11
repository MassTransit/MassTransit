---

title: CancelScheduledSendExtensions

---

# CancelScheduledSendExtensions

Namespace: MassTransit

```csharp
public static class CancelScheduledSendExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CancelScheduledSendExtensions](../masstransit/cancelscheduledsendextensions)

## Methods

### **CancelScheduledSend\<T\>(IMessageScheduler, ScheduledMessage\<T\>)**

Cancel a scheduled message

```csharp
public static Task CancelScheduledSend<T>(IMessageScheduler scheduler, ScheduledMessage<T> message)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`scheduler` [IMessageScheduler](../masstransit/imessagescheduler)<br/>
The message scheduler

`message` [ScheduledMessage\<T\>](../masstransit/scheduledmessage-1)<br/>
The

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CancelScheduledSend\<T\>(ConsumeContext, ScheduledMessage\<T\>)**

Cancel a scheduled message

```csharp
public static Task CancelScheduledSend<T>(ConsumeContext context, ScheduledMessage<T> message)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The message scheduler

`message` [ScheduledMessage\<T\>](../masstransit/scheduledmessage-1)<br/>
The

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
