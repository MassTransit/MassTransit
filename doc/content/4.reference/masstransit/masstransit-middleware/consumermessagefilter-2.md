---

title: ConsumerMessageFilter<TConsumer, TMessage>

---

# ConsumerMessageFilter\<TConsumer, TMessage\>

Namespace: MassTransit.Middleware

Consumes a message via Consumer, resolved through the consumer factory and notifies the context that the message was consumed.

```csharp
public class ConsumerMessageFilter<TConsumer, TMessage> : IFilter<ConsumeContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TConsumer`<br/>
The consumer type

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerMessageFilter\<TConsumer, TMessage\>](../masstransit-middleware/consumermessagefilter-2)<br/>
Implements [IFilter\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ConsumerMessageFilter(IConsumerFactory\<TConsumer\>, IPipe\<ConsumerConsumeContext\<TConsumer, TMessage\>\>)**

```csharp
public ConsumerMessageFilter(IConsumerFactory<TConsumer> consumerFactory, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> consumerPipe)
```

#### Parameters

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

`consumerPipe` [IPipe\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
