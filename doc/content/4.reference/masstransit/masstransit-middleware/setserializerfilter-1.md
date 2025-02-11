---

title: SetSerializerFilter<T>

---

# SetSerializerFilter\<T\>

Namespace: MassTransit.Middleware

Sets the CorrelationId header uses the supplied implementation.

```csharp
public class SetSerializerFilter<T> : IFilter<SendContext<T>>, IProbeSite
```

#### Type Parameters

`T`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SetSerializerFilter\<T\>](../masstransit-middleware/setserializerfilter-1)<br/>
Implements [IFilter\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **SetSerializerFilter(ContentType)**

```csharp
public SetSerializerFilter(ContentType contentType)
```

#### Parameters

`contentType` ContentType<br/>

## Methods

### **Send(SendContext\<T\>, IPipe\<SendContext\<T\>\>)**

```csharp
public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
```

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

`next` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
