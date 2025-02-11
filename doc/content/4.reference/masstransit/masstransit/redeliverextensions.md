---

title: RedeliverExtensions

---

# RedeliverExtensions

Namespace: MassTransit

```csharp
public static class RedeliverExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RedeliverExtensions](../masstransit/redeliverextensions)

## Methods

### **Redeliver\<T\>(ConsumeContext\<T\>, TimeSpan, Action\<ConsumeContext, SendContext\>)**

Redeliver uses the message scheduler to deliver the message to the queue at a future
 time. The delivery count is incremented. Moreover, if you give custom callback action, it perform before sending message to queue.
 A message scheduler must be configured on the bus for redelivery to be enabled.

```csharp
public static Task Redeliver<T>(ConsumeContext<T> context, TimeSpan delay, Action<ConsumeContext, SendContext> callback)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>
The consume context of the message

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The delay before the message is delivered. It may take longer to receive the message if the queue is not empty.

`callback` [Action\<ConsumeContext, SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
Operation which is executed before the message is delivered.

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
