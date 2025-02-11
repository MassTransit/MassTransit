---

title: ForwardMessagePipe<TMessage>

---

# ForwardMessagePipe\<TMessage\>

Namespace: MassTransit.Serialization

```csharp
public class ForwardMessagePipe<TMessage> : IPipe<SendContext<TMessage>>, IProbeSite, ISendPipe, ISendContextPipe
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ForwardMessagePipe\<TMessage\>](../masstransit-serialization/forwardmessagepipe-1)<br/>
Implements [IPipe\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [ISendPipe](../../masstransit-abstractions/masstransit-transports/isendpipe), [ISendContextPipe](../../masstransit-abstractions/masstransit-transports/isendcontextpipe)

## Constructors

### **ForwardMessagePipe(ConsumeContext\<TMessage\>, IPipe\<SendContext\<TMessage\>\>)**

```csharp
public ForwardMessagePipe(ConsumeContext<TMessage> context, IPipe<SendContext<TMessage>> pipe)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`pipe` [IPipe\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Send(SendContext\<TMessage\>)**

```csharp
public Task Send(SendContext<TMessage> context)
```

#### Parameters

`context` [SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send\<T\>(SendContext\<T\>)**

```csharp
public Task Send<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
