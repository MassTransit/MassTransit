---

title: MissingInstanceRedeliveryPipe<TSaga, TMessage>

---

# MissingInstanceRedeliveryPipe\<TSaga, TMessage\>

Namespace: MassTransit.Middleware

```csharp
public class MissingInstanceRedeliveryPipe<TSaga, TMessage> : IPipe<ConsumeContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MissingInstanceRedeliveryPipe\<TSaga, TMessage\>](../masstransit-middleware/missinginstanceredeliverypipe-2)<br/>
Implements [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **MissingInstanceRedeliveryPipe(IRetryPolicy, IPipe\<ConsumeContext\<TMessage\>\>, RedeliveryOptions)**

```csharp
public MissingInstanceRedeliveryPipe(IRetryPolicy retryPolicy, IPipe<ConsumeContext<TMessage>> finalPipe, RedeliveryOptions options)
```

#### Parameters

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

`finalPipe` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`options` [RedeliveryOptions](../../masstransit-abstractions/masstransit/redeliveryoptions)<br/>

## Methods

### **Send(ConsumeContext\<TMessage\>)**

```csharp
public Task Send(ConsumeContext<TMessage> context)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
