---

title: IPublishTopology

---

# IPublishTopology

Namespace: MassTransit

```csharp
public interface IPublishTopology : IPublishTopologyConfigurationObserverConnector
```

Implements [IPublishTopologyConfigurationObserverConnector](../masstransit-configuration/ipublishtopologyconfigurationobserverconnector)

## Methods

### **GetMessageTopology\<T\>()**

Returns the specification for the message type

```csharp
IMessagePublishTopology<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>
The message type

#### Returns

[IMessagePublishTopology\<T\>](../masstransit/imessagepublishtopology-1)<br/>

### **GetMessageTopology(Type)**

Returns the specification for the message type

```csharp
IMessagePublishTopology GetMessageTopology(Type messageType)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IMessagePublishTopology](../masstransit/imessagepublishtopology)<br/>

### **TryGetPublishAddress(Type, Uri, Uri)**

Returns the publish address for the message, using the topology rules. This cannot use
 a PublishContext because the transport isn't available yet.

```csharp
bool TryGetPublishAddress(Type messageType, Uri baseAddress, out Uri publishAddress)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The message type

`baseAddress` Uri<br/>
The host base address, used to build out the exchange address

`publishAddress` Uri<br/>
The address where the publish endpoint should send the message

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
true if the address was available, otherwise false
