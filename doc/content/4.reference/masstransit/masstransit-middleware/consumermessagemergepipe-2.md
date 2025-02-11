---

title: ConsumerMessageMergePipe<TConsumer, TMessage>

---

# ConsumerMessageMergePipe\<TConsumer, TMessage\>

Namespace: MassTransit.Middleware

Merges the out-of-band consumer back into the context

```csharp
public class ConsumerMessageMergePipe<TConsumer, TMessage> : IPipe<ConsumeContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerMessageMergePipe\<TConsumer, TMessage\>](../masstransit-middleware/consumermessagemergepipe-2)<br/>
Implements [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ConsumerMessageMergePipe(IPipe\<ConsumerConsumeContext\<TConsumer, TMessage\>\>, ConsumerConsumeContext\<TConsumer, TMessage\>)**

```csharp
public ConsumerMessageMergePipe(IPipe<ConsumerConsumeContext<TConsumer, TMessage>> output, ConsumerConsumeContext<TConsumer, TMessage> context)
```

#### Parameters

`output` [IPipe\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`context` [ConsumerConsumeContext\<TConsumer, TMessage\>](../../masstransit-abstractions/masstransit/consumerconsumecontext-2)<br/>

## Methods

### **Send(ConsumeContext\<TMessage\>)**

```csharp
public Task Send(ConsumeContext<TMessage> context)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
