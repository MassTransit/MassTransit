---

title: IConsumeTopology

---

# IConsumeTopology

Namespace: MassTransit

```csharp
public interface IConsumeTopology : IConsumeTopologyConfigurationObserverConnector
```

Implements [IConsumeTopologyConfigurationObserverConnector](../masstransit-configuration/iconsumetopologyconfigurationobserverconnector)

## Methods

### **GetMessageTopology\<T\>()**

Returns the specification for the message type

```csharp
IMessageConsumeTopology<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>
The message type

#### Returns

[IMessageConsumeTopology\<T\>](../masstransit/imessageconsumetopology-1)<br/>

### **CreateTemporaryQueueName(String)**

Create a temporary endpoint name, using the specified tag

```csharp
string CreateTemporaryQueueName(string tag)
```

#### Parameters

`tag` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
