---

title: InstanceMessageFilter<TConsumer, TMessage>

---

# InstanceMessageFilter\<TConsumer, TMessage\>

Namespace: MassTransit.Middleware

Consumes a message via an existing class instance

```csharp
public class InstanceMessageFilter<TConsumer, TMessage> : IFilter<ConsumeContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InstanceMessageFilter\<TConsumer, TMessage\>](../masstransit-middleware/instancemessagefilter-2)<br/>
Implements [IFilter\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **InstanceMessageFilter(TConsumer, IPipe\<ConsumerConsumeContext\<TConsumer, TMessage\>\>)**

```csharp
public InstanceMessageFilter(TConsumer instance, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> instancePipe)
```

#### Parameters

`instance` TConsumer<br/>

`instancePipe` [IPipe\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
