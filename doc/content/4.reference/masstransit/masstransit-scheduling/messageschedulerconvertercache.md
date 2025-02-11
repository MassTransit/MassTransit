---

title: MessageSchedulerConverterCache

---

# MessageSchedulerConverterCache

Namespace: MassTransit.Scheduling

Caches the converters that allow a raw object to be published using the object's type through
 the generic Send method.

```csharp
public class MessageSchedulerConverterCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageSchedulerConverterCache](../masstransit-scheduling/messageschedulerconvertercache)

## Constructors

### **MessageSchedulerConverterCache()**

```csharp
public MessageSchedulerConverterCache()
```

## Methods

### **ScheduleSend(IMessageScheduler, Uri, DateTime, Object, Type, CancellationToken)**

```csharp
public static Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, DateTime scheduledTime, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`scheduler` [IMessageScheduler](../../masstransit-abstractions/masstransit/imessagescheduler)<br/>

`destinationAddress` Uri<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleSend(IMessageScheduler, Uri, DateTime, Object, Type, IPipe\<SendContext\>, CancellationToken)**

```csharp
public static Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`scheduler` [IMessageScheduler](../../masstransit-abstractions/masstransit/imessagescheduler)<br/>

`destinationAddress` Uri<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringSend(IRecurringMessageScheduler, Uri, RecurringSchedule, Object, Type, IPipe\<SendContext\>, CancellationToken)**

```csharp
public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(IRecurringMessageScheduler scheduler, Uri destinationAddress, RecurringSchedule schedule, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`scheduler` [IRecurringMessageScheduler](../../masstransit-abstractions/masstransit/irecurringmessagescheduler)<br/>

`destinationAddress` Uri<br/>

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleRecurringSend(IRecurringMessageScheduler, Uri, RecurringSchedule, Object, Type, CancellationToken)**

```csharp
public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(IRecurringMessageScheduler scheduler, Uri destinationAddress, RecurringSchedule schedule, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`scheduler` [IRecurringMessageScheduler](../../masstransit-abstractions/masstransit/irecurringmessagescheduler)<br/>

`destinationAddress` Uri<br/>

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledRecurringMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
