---

title: SendContextPipeAdapter<TMessage>

---

# SendContextPipeAdapter\<TMessage\>

Namespace: MassTransit.Transports

```csharp
public abstract class SendContextPipeAdapter<TMessage> : IPipe<SendContext<TMessage>>, IProbeSite, ISendPipe, ISendContextPipe
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendContextPipeAdapter\<TMessage\>](../masstransit-transports/sendcontextpipeadapter-1)<br/>
Implements [IPipe\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [ISendPipe](../../masstransit-abstractions/masstransit-transports/isendpipe), [ISendContextPipe](../../masstransit-abstractions/masstransit-transports/isendcontextpipe)

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(SendContext\<TMessage\>)**

```csharp
protected abstract void Send(SendContext<TMessage> context)
```

#### Parameters

`context` [SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

### **Send\<T\>(SendContext\<T\>)**

```csharp
protected abstract void Send<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>
