---

title: MethodConsumerMessageFilter<TConsumer, TMessage>

---

# MethodConsumerMessageFilter\<TConsumer, TMessage\>

Namespace: MassTransit.Middleware

Dispatches the ConsumeContext to the consumer method for the specified message type

```csharp
public class MethodConsumerMessageFilter<TConsumer, TMessage> : IConsumerMessageFilter<TConsumer, TMessage>, IFilter<ConsumerConsumeContext<TConsumer, TMessage>>, IProbeSite
```

#### Type Parameters

`TConsumer`<br/>
The consumer type

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MethodConsumerMessageFilter\<TConsumer, TMessage\>](../masstransit-middleware/methodconsumermessagefilter-2)<br/>
Implements [IConsumerMessageFilter\<TConsumer, TMessage\>](../masstransit-middleware/iconsumermessagefilter-2), [IFilter\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **MethodConsumerMessageFilter()**

```csharp
public MethodConsumerMessageFilter()
```
