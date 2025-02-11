---

title: ConsumeSendPipeAdapter<TMessage>

---

# ConsumeSendPipeAdapter\<TMessage\>

Namespace: MassTransit.Transports

```csharp
public class ConsumeSendPipeAdapter<TMessage> : SendContextPipeAdapter<TMessage>, IPipe<SendContext<TMessage>>, IProbeSite, ISendPipe, ISendContextPipe
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendContextPipeAdapter\<TMessage\>](../masstransit-transports/sendcontextpipeadapter-1) → [ConsumeSendPipeAdapter\<TMessage\>](../masstransit-transports/consumesendpipeadapter-1)<br/>
Implements [IPipe\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [ISendPipe](../../masstransit-abstractions/masstransit-transports/isendpipe), [ISendContextPipe](../../masstransit-abstractions/masstransit-transports/isendcontextpipe)

## Constructors

### **ConsumeSendPipeAdapter(ConsumeContext, IPipe\<SendContext\<TMessage\>\>, Nullable\<Guid\>)**

```csharp
public ConsumeSendPipeAdapter(ConsumeContext consumeContext, IPipe<SendContext<TMessage>> pipe, Nullable<Guid> requestId)
```

#### Parameters

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`pipe` [IPipe\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`requestId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **Send\<T\>(SendContext\<T\>)**

```csharp
protected void Send<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

### **Send(SendContext\<TMessage\>)**

```csharp
protected void Send(SendContext<TMessage> context)
```

#### Parameters

`context` [SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>
