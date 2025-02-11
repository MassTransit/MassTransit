---

title: MessageRedeliveryContext

---

# MessageRedeliveryContext

Namespace: MassTransit

Used to reschedule delivery of the current message

```csharp
public interface MessageRedeliveryContext
```

## Methods

### **ScheduleRedelivery(TimeSpan, Action\<ConsumeContext, SendContext\>)**

Schedule the message to be redelivered after the specified delay with given operation.

```csharp
Task ScheduleRedelivery(TimeSpan delay, Action<ConsumeContext, SendContext> callback)
```

#### Parameters

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The minimum delay before the message will be redelivered to the queue

`callback` [Action\<ConsumeContext, SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
Operation which perform during message redeliver to queue

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
