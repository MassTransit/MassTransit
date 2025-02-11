---

title: IMessageConsumeTopologyConvention<TMessage>

---

# IMessageConsumeTopologyConvention\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public interface IMessageConsumeTopologyConvention<TMessage> : IMessageConsumeTopologyConvention
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessageConsumeTopologyConvention](../masstransit-configuration/imessageconsumetopologyconvention)

## Methods

### **TryGetMessageConsumeTopology(IMessageConsumeTopology\<TMessage\>)**

```csharp
bool TryGetMessageConsumeTopology(out IMessageConsumeTopology<TMessage> messageConsumeTopology)
```

#### Parameters

`messageConsumeTopology` [IMessageConsumeTopology\<TMessage\>](../masstransit/imessageconsumetopology-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
