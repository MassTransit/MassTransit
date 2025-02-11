---

title: IMessagePublishTopologyConvention<TMessage>

---

# IMessagePublishTopologyConvention\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public interface IMessagePublishTopologyConvention<TMessage> : IMessagePublishTopologyConvention
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessagePublishTopologyConvention](../masstransit-configuration/imessagepublishtopologyconvention)

## Methods

### **TryGetMessagePublishTopology(IMessagePublishTopology\<TMessage\>)**

```csharp
bool TryGetMessagePublishTopology(out IMessagePublishTopology<TMessage> messagePublishTopology)
```

#### Parameters

`messagePublishTopology` [IMessagePublishTopology\<TMessage\>](../masstransit/imessagepublishtopology-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
