---

title: ISagaMessageConfigurator<TSaga, TMessage>

---

# ISagaMessageConfigurator\<TSaga, TMessage\>

Namespace: MassTransit

```csharp
public interface ISagaMessageConfigurator<TSaga, TMessage> : IPipeConfigurator<SagaConsumeContext<TSaga, TMessage>>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Implements [IPipeConfigurator\<SagaConsumeContext\<TSaga, TMessage\>\>](../masstransit/ipipeconfigurator-1)

## Methods

### **Message(Action\<ISagaMessageConfigurator\<TMessage\>\>)**

Add middleware to the saga pipeline, for the specified message type, which is
 invoked after the saga repository.

```csharp
void Message(Action<ISagaMessageConfigurator<TMessage>> configure)
```

#### Parameters

`configure` [Action\<ISagaMessageConfigurator\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback to configure the message pipeline
