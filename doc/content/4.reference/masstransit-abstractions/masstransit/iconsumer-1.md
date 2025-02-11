---

title: IConsumer<TMessage>

---

# IConsumer\<TMessage\>

Namespace: MassTransit

Defines a class that is a consumer of a message. The message is wrapped in an IConsumeContext
 interface to allow access to details surrounding the inbound message, including headers.

```csharp
public interface IConsumer<TMessage> : IConsumer
```

#### Type Parameters

`TMessage`<br/>
The message type

Implements [IConsumer](../masstransit/iconsumer)

## Methods

### **Consume(ConsumeContext\<TMessage\>)**

```csharp
Task Consume(ConsumeContext<TMessage> context)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
