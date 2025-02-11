---

title: IMessageFabric<TContext, T>

---

# IMessageFabric\<TContext, T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public interface IMessageFabric<TContext, T> : IMessageFabricObserverConnector<TContext>, IAgent, IProbeSite
```

#### Type Parameters

`TContext`<br/>

`T`<br/>

Implements [IMessageFabricObserverConnector\<TContext\>](../masstransit-transports-fabric/imessagefabricobserverconnector-1), [IAgent](../../masstransit-abstractions/masstransit/iagent), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **ExchangeDeclare(TContext, String, ExchangeType)**

```csharp
void ExchangeDeclare(TContext context, string name, ExchangeType exchangeType)
```

#### Parameters

`context` TContext<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

### **ExchangeBind(TContext, String, String, String)**

```csharp
void ExchangeBind(TContext context, string source, string destination, string routingKey)
```

#### Parameters

`context` TContext<br/>

`source` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`destination` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **QueueDeclare(TContext, String)**

```csharp
void QueueDeclare(TContext context, string name)
```

#### Parameters

`context` TContext<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **QueueBind(TContext, String, String)**

```csharp
void QueueBind(TContext context, string source, string destination)
```

#### Parameters

`context` TContext<br/>

`source` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`destination` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetExchange(TContext, String, ExchangeType)**

```csharp
IMessageExchange<T> GetExchange(TContext context, string name, ExchangeType exchangeType)
```

#### Parameters

`context` TContext<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

#### Returns

[IMessageExchange\<T\>](../masstransit-transports-fabric/imessageexchange-1)<br/>

### **GetQueue(TContext, String)**

```csharp
IMessageQueue<TContext, T> GetQueue(TContext context, string name)
```

#### Parameters

`context` TContext<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IMessageQueue\<TContext, T\>](../masstransit-transports-fabric/imessagequeue-2)<br/>
