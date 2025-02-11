---

title: IMessagePublishTopology

---

# IMessagePublishTopology

Namespace: MassTransit

```csharp
public interface IMessagePublishTopology
```

## Properties

### **Exclude**

True if the message type should be excluded from the broker topology

```csharp
public abstract bool Exclude { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **TryGetPublishAddress(Uri, Uri)**

Returns the publish address for the message, using the topology rules. This cannot use
 a PublishContext because the transport isn't available yet.

```csharp
bool TryGetPublishAddress(Uri baseAddress, out Uri publishAddress)
```

#### Parameters

`baseAddress` Uri<br/>
The host base address, used to build out the exchange address

`publishAddress` Uri<br/>
The address where the publish endpoint should send the message

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
true if the address was available, otherwise false
