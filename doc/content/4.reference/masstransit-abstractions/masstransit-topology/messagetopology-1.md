---

title: MessageTopology<TMessage>

---

# MessageTopology\<TMessage\>

Namespace: MassTransit.Topology

```csharp
public class MessageTopology<TMessage> : IMessageTopologyConfigurator<TMessage>, IMessageTypeTopologyConfigurator, IMessageTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageTopology\<TMessage\>](../masstransit-topology/messagetopology-1)<br/>
Implements [IMessageTopologyConfigurator\<TMessage\>](../masstransit-configuration/imessagetopologyconfigurator-1), [IMessageTypeTopologyConfigurator](../masstransit-configuration/imessagetypetopologyconfigurator), [IMessageTopology\<TMessage\>](../masstransit/imessagetopology-1)

## Properties

### **EntityNameFormatter**

```csharp
public IMessageEntityNameFormatter<TMessage> EntityNameFormatter { get; private set; }
```

#### Property Value

[IMessageEntityNameFormatter\<TMessage\>](../masstransit/imessageentitynameformatter-1)<br/>

### **EntityName**

```csharp
public string EntityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **MessageTopology(IMessageEntityNameFormatter\<TMessage\>)**

```csharp
public MessageTopology(IMessageEntityNameFormatter<TMessage> entityNameFormatter)
```

#### Parameters

`entityNameFormatter` [IMessageEntityNameFormatter\<TMessage\>](../masstransit/imessageentitynameformatter-1)<br/>

## Methods

### **SetEntityNameFormatter(IMessageEntityNameFormatter\<TMessage\>)**

```csharp
public void SetEntityNameFormatter(IMessageEntityNameFormatter<TMessage> entityNameFormatter)
```

#### Parameters

`entityNameFormatter` [IMessageEntityNameFormatter\<TMessage\>](../masstransit/imessageentitynameformatter-1)<br/>

### **SetEntityName(String)**

```csharp
public void SetEntityName(string entityName)
```

#### Parameters

`entityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
