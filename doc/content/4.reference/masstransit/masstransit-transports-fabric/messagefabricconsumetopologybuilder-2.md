---

title: MessageFabricConsumeTopologyBuilder<TContext, T>

---

# MessageFabricConsumeTopologyBuilder\<TContext, T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public class MessageFabricConsumeTopologyBuilder<TContext, T> : IMessageFabricConsumeTopologyBuilder, IMessageFabricTopologyBuilder
```

#### Type Parameters

`TContext`<br/>

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageFabricConsumeTopologyBuilder\<TContext, T\>](../masstransit-transports-fabric/messagefabricconsumetopologybuilder-2)<br/>
Implements [IMessageFabricConsumeTopologyBuilder](../masstransit-configuration/imessagefabricconsumetopologybuilder), [IMessageFabricTopologyBuilder](../masstransit-configuration/imessagefabrictopologybuilder)

## Properties

### **Exchange**

```csharp
public string Exchange { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Queue**

```csharp
public string Queue { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **MessageFabricConsumeTopologyBuilder(TContext, IMessageFabric\<TContext, T\>)**

```csharp
public MessageFabricConsumeTopologyBuilder(TContext context, IMessageFabric<TContext, T> fabric)
```

#### Parameters

`context` TContext<br/>

`fabric` [IMessageFabric\<TContext, T\>](../masstransit-transports-fabric/imessagefabric-2)<br/>

## Methods

### **ExchangeBind(String, String, String)**

```csharp
public void ExchangeBind(string source, string destination, string routingKey)
```

#### Parameters

`source` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`destination` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **QueueBind(String, String)**

```csharp
public void QueueBind(string source, string destination)
```

#### Parameters

`source` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`destination` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ExchangeDeclare(String, ExchangeType)**

```csharp
public void ExchangeDeclare(string name, ExchangeType exchangeType)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

### **QueueDeclare(String)**

```csharp
public void QueueDeclare(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
