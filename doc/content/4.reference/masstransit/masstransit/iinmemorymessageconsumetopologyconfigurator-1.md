---

title: IInMemoryMessageConsumeTopologyConfigurator<TMessage>

---

# IInMemoryMessageConsumeTopologyConfigurator\<TMessage\>

Namespace: MassTransit

```csharp
public interface IInMemoryMessageConsumeTopologyConfigurator<TMessage> : IMessageConsumeTopologyConfigurator<TMessage>, IMessageConsumeTopologyConfigurator, ISpecification, IMessageConsumeTopology<TMessage>, IInMemoryMessageConsumeTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessageConsumeTopologyConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator-1), [IMessageConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IMessageConsumeTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessageconsumetopology-1), [IInMemoryMessageConsumeTopology\<TMessage\>](../masstransit/iinmemorymessageconsumetopology-1)

## Methods

### **Bind(Nullable\<ExchangeType\>, String)**

Adds the exchange bindings for this message type

```csharp
void Bind(Nullable<ExchangeType> exchangeType, string routingKey)
```

#### Parameters

`exchangeType` [Nullable\<ExchangeType\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
