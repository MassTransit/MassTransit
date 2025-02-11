---

title: ScheduleMessageRedeliveryContext<TMessage>

---

# ScheduleMessageRedeliveryContext\<TMessage\>

Namespace: MassTransit.Context

Used to schedule message redelivery using the message scheduler

```csharp
public class ScheduleMessageRedeliveryContext<TMessage> : MessageRedeliveryContext
```

#### Type Parameters

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduleMessageRedeliveryContext\<TMessage\>](../masstransit-context/schedulemessageredeliverycontext-1)<br/>
Implements [MessageRedeliveryContext](../../masstransit-abstractions/masstransit/messageredeliverycontext)

## Constructors

### **ScheduleMessageRedeliveryContext(ConsumeContext\<TMessage\>, RedeliveryOptions)**

```csharp
public ScheduleMessageRedeliveryContext(ConsumeContext<TMessage> context, RedeliveryOptions options)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`options` [RedeliveryOptions](../../masstransit-abstractions/masstransit/redeliveryoptions)<br/>

## Methods

### **ScheduleRedelivery(TimeSpan, Action\<ConsumeContext, SendContext\>)**

```csharp
public Task ScheduleRedelivery(TimeSpan delay, Action<ConsumeContext, SendContext> callback)
```

#### Parameters

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`callback` [Action\<ConsumeContext, SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
