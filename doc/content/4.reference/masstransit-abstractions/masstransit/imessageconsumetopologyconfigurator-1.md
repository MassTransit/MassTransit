---

title: IMessageConsumeTopologyConfigurator<TMessage>

---

# IMessageConsumeTopologyConfigurator\<TMessage\>

Namespace: MassTransit

Configures the Consuming of a message type, allowing filters to be applied
 on Consume.

```csharp
public interface IMessageConsumeTopologyConfigurator<TMessage> : IMessageConsumeTopologyConfigurator, ISpecification, IMessageConsumeTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessageConsumeTopologyConfigurator](../masstransit/imessageconsumetopologyconfigurator), [ISpecification](../masstransit/ispecification), [IMessageConsumeTopology\<TMessage\>](../masstransit/imessageconsumetopology-1)

## Methods

### **Add(IMessageConsumeTopology\<TMessage\>)**

```csharp
void Add(IMessageConsumeTopology<TMessage> consumeTopology)
```

#### Parameters

`consumeTopology` [IMessageConsumeTopology\<TMessage\>](../masstransit/imessageconsumetopology-1)<br/>

### **AddDelegate(IMessageConsumeTopology\<TMessage\>)**

Adds a delegated configuration to the Consume topology, which is called before any topologies
 in this configuration.

```csharp
void AddDelegate(IMessageConsumeTopology<TMessage> configuration)
```

#### Parameters

`configuration` [IMessageConsumeTopology\<TMessage\>](../masstransit/imessageconsumetopology-1)<br/>

### **TryAddConvention(IMessageConsumeTopologyConvention\<TMessage\>)**

Adds a convention to the message Consume topology configuration, which can be modified

```csharp
bool TryAddConvention(IMessageConsumeTopologyConvention<TMessage> convention)
```

#### Parameters

`convention` [IMessageConsumeTopologyConvention\<TMessage\>](../masstransit-configuration/imessageconsumetopologyconvention-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **UpdateConvention\<TConvention\>(Func\<TConvention, TConvention\>)**

Update a convention if available, otherwise, throw an exception

```csharp
void UpdateConvention<TConvention>(Func<TConvention, TConvention> update)
```

#### Type Parameters

`TConvention`<br/>

#### Parameters

`update` [Func\<TConvention, TConvention\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Called if the convention already exists

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
