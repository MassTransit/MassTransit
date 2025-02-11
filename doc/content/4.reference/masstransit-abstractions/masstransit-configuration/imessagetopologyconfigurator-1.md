---

title: IMessageTopologyConfigurator<TMessage>

---

# IMessageTopologyConfigurator\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public interface IMessageTopologyConfigurator<TMessage> : IMessageTypeTopologyConfigurator, IMessageTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessageTypeTopologyConfigurator](../masstransit-configuration/imessagetypetopologyconfigurator), [IMessageTopology\<TMessage\>](../masstransit/imessagetopology-1)

## Methods

### **SetEntityNameFormatter(IMessageEntityNameFormatter\<TMessage\>)**

Sets the entity name formatter used for this message type

```csharp
void SetEntityNameFormatter(IMessageEntityNameFormatter<TMessage> entityNameFormatter)
```

#### Parameters

`entityNameFormatter` [IMessageEntityNameFormatter\<TMessage\>](../masstransit/imessageentitynameformatter-1)<br/>

### **SetEntityName(String)**

Sets the entity name for this message type

```csharp
void SetEntityName(string entityName)
```

#### Parameters

`entityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The entity name
