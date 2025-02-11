---

title: MessageHandlerConsumer<T, T1, T2, T3>

---

# MessageHandlerConsumer\<T, T1, T2, T3\>

Namespace: MassTransit.DependencyInjection

```csharp
public class MessageHandlerConsumer<T, T1, T2, T3> : IConsumer<T>, IConsumer
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageHandlerConsumer\<T, T1, T2, T3\>](../masstransit-dependencyinjection/messagehandlerconsumer-4)<br/>
Implements [IConsumer\<T\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer)

## Constructors

### **MessageHandlerConsumer(MessageHandlerMethod\<T, T1, T2, T3\>, T1, T2, T3)**

```csharp
public MessageHandlerConsumer(MessageHandlerMethod<T, T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3)
```

#### Parameters

`method` [MessageHandlerMethod\<T, T1, T2, T3\>](../masstransit-dependencyinjection/messagehandlermethod-4)<br/>

`arg1` T1<br/>

`arg2` T2<br/>

`arg3` T3<br/>

## Methods

### **Consume(ConsumeContext\<T\>)**

```csharp
public Task Consume(ConsumeContext<T> context)
```

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
