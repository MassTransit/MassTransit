---

title: MessageHandlerConsumer<T, T1, T2>

---

# MessageHandlerConsumer\<T, T1, T2\>

Namespace: MassTransit.DependencyInjection

```csharp
public class MessageHandlerConsumer<T, T1, T2> : IConsumer<T>, IConsumer
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageHandlerConsumer\<T, T1, T2\>](../masstransit-dependencyinjection/messagehandlerconsumer-3)<br/>
Implements [IConsumer\<T\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer)

## Constructors

### **MessageHandlerConsumer(MessageHandlerMethod\<T, T1, T2\>, T1, T2)**

```csharp
public MessageHandlerConsumer(MessageHandlerMethod<T, T1, T2> method, T1 arg1, T2 arg2)
```

#### Parameters

`method` [MessageHandlerMethod\<T, T1, T2\>](../masstransit-dependencyinjection/messagehandlermethod-3)<br/>

`arg1` T1<br/>

`arg2` T2<br/>

## Methods

### **Consume(ConsumeContext\<T\>)**

```csharp
public Task Consume(ConsumeContext<T> context)
```

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
