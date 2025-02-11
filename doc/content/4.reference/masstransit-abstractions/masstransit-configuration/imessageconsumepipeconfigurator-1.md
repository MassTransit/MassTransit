---

title: IMessageConsumePipeConfigurator<TMessage>

---

# IMessageConsumePipeConfigurator\<TMessage\>

Namespace: MassTransit.Configuration

Configures the Consuming of a message type, allowing filters to be applied
 on Consume.

```csharp
public interface IMessageConsumePipeConfigurator<TMessage> : IPipeConfigurator<ConsumeContext<TMessage>>
```

#### Type Parameters

`TMessage`<br/>

Implements [IPipeConfigurator\<ConsumeContext\<TMessage\>\>](../masstransit/ipipeconfigurator-1)
