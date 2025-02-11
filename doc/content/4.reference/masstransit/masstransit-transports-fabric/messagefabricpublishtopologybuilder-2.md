---

title: MessageFabricPublishTopologyBuilder<TContext, T>

---

# MessageFabricPublishTopologyBuilder\<TContext, T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public class MessageFabricPublishTopologyBuilder<TContext, T> : IMessageFabricPublishTopologyBuilder, IMessageFabricTopologyBuilder
```

#### Type Parameters

`TContext`<br/>

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageFabricPublishTopologyBuilder\<TContext, T\>](../masstransit-transports-fabric/messagefabricpublishtopologybuilder-2)<br/>
Implements [IMessageFabricPublishTopologyBuilder](../masstransit-configuration/imessagefabricpublishtopologybuilder), [IMessageFabricTopologyBuilder](../masstransit-configuration/imessagefabrictopologybuilder)

## Properties

### **ExchangeName**

```csharp
public string ExchangeName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ExchangeType**

```csharp
public ExchangeType ExchangeType { get; set; }
```

#### Property Value

[ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

## Constructors

### **MessageFabricPublishTopologyBuilder(TContext, IMessageFabric\<TContext, T\>)**

```csharp
public MessageFabricPublishTopologyBuilder(TContext context, IMessageFabric<TContext, T> messageFabric)
```

#### Parameters

`context` TContext<br/>

`messageFabric` [IMessageFabric\<TContext, T\>](../masstransit-transports-fabric/imessagefabric-2)<br/>

## Methods

### **CreateImplementedBuilder()**

```csharp
public IMessageFabricPublishTopologyBuilder CreateImplementedBuilder()
```

#### Returns

[IMessageFabricPublishTopologyBuilder](../masstransit-configuration/imessagefabricpublishtopologybuilder)<br/>

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
