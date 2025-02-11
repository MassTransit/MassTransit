---

title: IMessagePublishTopologyConfigurator<TMessage>

---

# IMessagePublishTopologyConfigurator\<TMessage\>

Namespace: MassTransit

Configures the Publishing of a message type, allowing filters to be applied
 on Publish.

```csharp
public interface IMessagePublishTopologyConfigurator<TMessage> : IMessagePublishTopologyConfigurator, IMessagePublishTopology, ISpecification, IMessagePublishTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessagePublishTopologyConfigurator](../masstransit/imessagepublishtopologyconfigurator), [IMessagePublishTopology](../masstransit/imessagepublishtopology), [ISpecification](../masstransit/ispecification), [IMessagePublishTopology\<TMessage\>](../masstransit/imessagepublishtopology-1)

## Methods

### **Add(IMessagePublishTopology\<TMessage\>)**

```csharp
void Add(IMessagePublishTopology<TMessage> publishTopology)
```

#### Parameters

`publishTopology` [IMessagePublishTopology\<TMessage\>](../masstransit/imessagepublishtopology-1)<br/>

### **AddDelegate(IMessagePublishTopology\<TMessage\>)**

Adds a delegated configuration to the Publish topology, which is called before any topologies
 in this configuration.

```csharp
void AddDelegate(IMessagePublishTopology<TMessage> configuration)
```

#### Parameters

`configuration` [IMessagePublishTopology\<TMessage\>](../masstransit/imessagepublishtopology-1)<br/>

### **TryAddConvention(IMessagePublishTopologyConvention\<TMessage\>)**

Adds a convention to the message Publish topology configuration, which can be modified

```csharp
bool TryAddConvention(IMessagePublishTopologyConvention<TMessage> convention)
```

#### Parameters

`convention` [IMessagePublishTopologyConvention\<TMessage\>](../masstransit-configuration/imessagepublishtopologyconvention-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AddOrUpdateConvention\<TConvention\>(Func\<TConvention\>, Func\<TConvention, TConvention\>)**

Returns the first convention that matches the interface type specified, to allow it to be customized
 and or replaced.

```csharp
void AddOrUpdateConvention<TConvention>(Func<TConvention> add, Func<TConvention, TConvention> update)
```

#### Type Parameters

`TConvention`<br/>

#### Parameters

`add` [Func\<TConvention\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>
Called if the convention does not already exist

`update` [Func\<TConvention, TConvention\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Called if the convention already exists
