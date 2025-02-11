---

title: DeserializeFilter

---

# DeserializeFilter

Namespace: MassTransit.Middleware

Performs the deserialization of a message ReceiveContext and passes the resulting
 ConsumeContext to the output pipe.

```csharp
public class DeserializeFilter : IFilter<ReceiveContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DeserializeFilter](../masstransit-middleware/deserializefilter)<br/>
Implements [IFilter\<ReceiveContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **DeserializeFilter(ISerialization, IPipe\<ConsumeContext\>)**

```csharp
public DeserializeFilter(ISerialization serializers, IPipe<ConsumeContext> output)
```

#### Parameters

`serializers` [ISerialization](../../masstransit-abstractions/masstransit/iserialization)<br/>

`output` [IPipe\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(ReceiveContext, IPipe\<ReceiveContext\>)**

```csharp
public Task Send(ReceiveContext context, IPipe<ReceiveContext> next)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

`next` [IPipe\<ReceiveContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
