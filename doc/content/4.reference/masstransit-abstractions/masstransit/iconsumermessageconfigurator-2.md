---

title: IConsumerMessageConfigurator<TConsumer, TMessage>

---

# IConsumerMessageConfigurator\<TConsumer, TMessage\>

Namespace: MassTransit

```csharp
public interface IConsumerMessageConfigurator<TConsumer, TMessage> : IPipeConfigurator<ConsumerConsumeContext<TConsumer, TMessage>>
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

Implements [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../masstransit/ipipeconfigurator-1)

## Methods

### **Message(Action\<IConsumerMessageConfigurator\<TMessage\>\>)**

Add middleware to the consumer pipeline, for the specified message type, which is
 invoked after the consumer factory.

```csharp
void Message(Action<IConsumerMessageConfigurator<TMessage>> configure)
```

#### Parameters

`configure` [Action\<IConsumerMessageConfigurator\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback to configure the message pipeline
