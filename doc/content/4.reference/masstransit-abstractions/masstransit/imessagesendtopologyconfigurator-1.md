---

title: IMessageSendTopologyConfigurator<TMessage>

---

# IMessageSendTopologyConfigurator\<TMessage\>

Namespace: MassTransit

Configures the sending of a message type, allowing filters to be applied
 on send.

```csharp
public interface IMessageSendTopologyConfigurator<TMessage> : IMessageSendTopologyConfigurator, ISpecification, IMessageSendTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessageSendTopologyConfigurator](../masstransit/imessagesendtopologyconfigurator), [ISpecification](../masstransit/ispecification), [IMessageSendTopology\<TMessage\>](../masstransit/imessagesendtopology-1)

## Methods

### **Add(IMessageSendTopology\<TMessage\>)**

```csharp
void Add(IMessageSendTopology<TMessage> sendTopology)
```

#### Parameters

`sendTopology` [IMessageSendTopology\<TMessage\>](../masstransit/imessagesendtopology-1)<br/>

### **AddDelegate(IMessageSendTopology\<TMessage\>)**

Adds a delegated configuration to the send topology, which is called before any topologies
 in this configuration.

```csharp
void AddDelegate(IMessageSendTopology<TMessage> configuration)
```

#### Parameters

`configuration` [IMessageSendTopology\<TMessage\>](../masstransit/imessagesendtopology-1)<br/>

### **TryAddConvention(IMessageSendTopologyConvention\<TMessage\>)**

Adds a convention to the message send topology configuration, which can be modified

```csharp
bool TryAddConvention(IMessageSendTopologyConvention<TMessage> convention)
```

#### Parameters

`convention` [IMessageSendTopologyConvention\<TMessage\>](../masstransit-configuration/imessagesendtopologyconvention-1)<br/>

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

### **TryGetConvention\<TConvention\>(TConvention)**

Returns the convention, if found

```csharp
bool TryGetConvention<TConvention>(out TConvention convention)
```

#### Type Parameters

`TConvention`<br/>

#### Parameters

`convention` TConvention<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
