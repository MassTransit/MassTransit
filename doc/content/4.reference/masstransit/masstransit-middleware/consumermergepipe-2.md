---

title: ConsumerMergePipe<TConsumer, TMessage>

---

# ConsumerMergePipe\<TConsumer, TMessage\>

Namespace: MassTransit.Middleware

Merges the out-of-band consumer back into the pipe

```csharp
public class ConsumerMergePipe<TConsumer, TMessage> : IPipe<ConsumerConsumeContext<TConsumer>>, IProbeSite
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerMergePipe\<TConsumer, TMessage\>](../masstransit-middleware/consumermergepipe-2)<br/>
Implements [IPipe\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ConsumerMergePipe(IPipe\<ConsumerConsumeContext\<TConsumer, TMessage\>\>)**

```csharp
public ConsumerMergePipe(IPipe<ConsumerConsumeContext<TConsumer, TMessage>> output)
```

#### Parameters

`output` [IPipe\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Send(ConsumerConsumeContext\<TConsumer\>)**

```csharp
public Task Send(ConsumerConsumeContext<TConsumer> context)
```

#### Parameters

`context` [ConsumerConsumeContext\<TConsumer\>](../../masstransit-abstractions/masstransit/consumerconsumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
