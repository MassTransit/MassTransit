---

title: ISqlMessageConsumeTopologyConfigurator<TMessage>

---

# ISqlMessageConsumeTopologyConfigurator\<TMessage\>

Namespace: MassTransit

```csharp
public interface ISqlMessageConsumeTopologyConfigurator<TMessage> : IMessageConsumeTopologyConfigurator<TMessage>, IMessageConsumeTopologyConfigurator, ISpecification, IMessageConsumeTopology<TMessage>, ISqlMessageConsumeTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessageConsumeTopologyConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator-1), [IMessageConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IMessageConsumeTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessageconsumetopology-1), [ISqlMessageConsumeTopology\<TMessage\>](../masstransit/isqlmessageconsumetopology-1)

## Methods

### **Subscribe(Action\<ISqlTopicSubscriptionConfigurator\>)**

Adds the exchange bindings for this message type

```csharp
void Subscribe(Action<ISqlTopicSubscriptionConfigurator> configure)
```

#### Parameters

`configure` [Action\<ISqlTopicSubscriptionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the binding and the exchange
