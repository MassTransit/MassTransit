---

title: IMessageFabricObserver<TContext>

---

# IMessageFabricObserver\<TContext\>

Namespace: MassTransit.Transports.Fabric

```csharp
public interface IMessageFabricObserver<TContext>
```

#### Type Parameters

`TContext`<br/>

## Methods

### **ExchangeDeclared(TContext, String, ExchangeType)**

```csharp
void ExchangeDeclared(TContext context, string name, ExchangeType exchangeType)
```

#### Parameters

`context` TContext<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

### **ExchangeBindingCreated(TContext, String, String, String)**

```csharp
void ExchangeBindingCreated(TContext context, string source, string destination, string routingKey)
```

#### Parameters

`context` TContext<br/>

`source` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`destination` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **QueueDeclared(TContext, String)**

```csharp
void QueueDeclared(TContext context, string name)
```

#### Parameters

`context` TContext<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **QueueBindingCreated(TContext, String, String)**

```csharp
void QueueBindingCreated(TContext context, string source, string destination)
```

#### Parameters

`context` TContext<br/>

`source` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`destination` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConsumerConnected(TContext, TopologyHandle, String)**

```csharp
TopologyHandle ConsumerConnected(TContext context, TopologyHandle handle, string queueName)
```

#### Parameters

`context` TContext<br/>

`handle` [TopologyHandle](../masstransit-transports-fabric/topologyhandle)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[TopologyHandle](../masstransit-transports-fabric/topologyhandle)<br/>
