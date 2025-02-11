---

title: DelayedMessageRedeliveryContext<TMessage>

---

# DelayedMessageRedeliveryContext\<TMessage\>

Namespace: MassTransit.Middleware

```csharp
public class DelayedMessageRedeliveryContext<TMessage> : MessageRedeliveryContext
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelayedMessageRedeliveryContext\<TMessage\>](../masstransit-middleware/delayedmessageredeliverycontext-1)<br/>
Implements [MessageRedeliveryContext](../../masstransit-abstractions/masstransit/messageredeliverycontext)

## Constructors

### **DelayedMessageRedeliveryContext(ConsumeContext\<TMessage\>, RedeliveryOptions)**

```csharp
public DelayedMessageRedeliveryContext(ConsumeContext<TMessage> context, RedeliveryOptions options)
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
