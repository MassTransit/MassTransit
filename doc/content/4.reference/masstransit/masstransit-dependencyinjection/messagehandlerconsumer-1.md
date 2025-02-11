---

title: MessageHandlerConsumer<T>

---

# MessageHandlerConsumer\<T\>

Namespace: MassTransit.DependencyInjection

```csharp
public class MessageHandlerConsumer<T> : IConsumer<T>, IConsumer
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageHandlerConsumer\<T\>](../masstransit-dependencyinjection/messagehandlerconsumer-1)<br/>
Implements [IConsumer\<T\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer)

## Constructors

### **MessageHandlerConsumer(MessageHandlerMethod\<T\>)**

```csharp
public MessageHandlerConsumer(MessageHandlerMethod<T> method)
```

#### Parameters

`method` [MessageHandlerMethod\<T\>](../masstransit-dependencyinjection/messagehandlermethod-1)<br/>

## Methods

### **Consume(ConsumeContext\<T\>)**

```csharp
public Task Consume(ConsumeContext<T> context)
```

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
