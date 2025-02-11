---

title: SetPartitionKeyFilter<TMessage>

---

# SetPartitionKeyFilter\<TMessage\>

Namespace: MassTransit.Middleware

```csharp
public class SetPartitionKeyFilter<TMessage> : IFilter<SendContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SetPartitionKeyFilter\<TMessage\>](../masstransit-middleware/setpartitionkeyfilter-1)<br/>
Implements [IFilter\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **SetPartitionKeyFilter(IMessagePartitionKeyFormatter\<TMessage\>)**

```csharp
public SetPartitionKeyFilter(IMessagePartitionKeyFormatter<TMessage> routingKeyFormatter)
```

#### Parameters

`routingKeyFormatter` [IMessagePartitionKeyFormatter\<TMessage\>](../masstransit-transports/imessagepartitionkeyformatter-1)<br/>

## Methods

### **Send(SendContext\<TMessage\>, IPipe\<SendContext\<TMessage\>\>)**

```csharp
public Task Send(SendContext<TMessage> context, IPipe<SendContext<TMessage>> next)
```

#### Parameters

`context` [SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

`next` [IPipe\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
