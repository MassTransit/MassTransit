---

title: MessageFabricObservable<TContext>

---

# MessageFabricObservable\<TContext\>

Namespace: MassTransit.Transports.Fabric

```csharp
public class MessageFabricObservable<TContext> : Connectable<IMessageFabricObserver<TContext>>, IMessageFabricObserver<TContext>
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IMessageFabricObserver\<TContext\>\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [MessageFabricObservable\<TContext\>](../masstransit-transports-fabric/messagefabricobservable-1)<br/>
Implements [IMessageFabricObserver\<TContext\>](../masstransit-transports-fabric/imessagefabricobserver-1)

## Properties

### **Connected**

```csharp
public IMessageFabricObserver`1[] Connected { get; }
```

#### Property Value

[IMessageFabricObserver`1[]](../masstransit-transports-fabric/imessagefabricobserver-1)<br/>

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **MessageFabricObservable()**

```csharp
public MessageFabricObservable()
```

## Methods

### **ExchangeDeclared(TContext, String, ExchangeType)**

```csharp
public void ExchangeDeclared(TContext context, string name, ExchangeType exchangeType)
```

#### Parameters

`context` TContext<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

### **ExchangeBindingCreated(TContext, String, String, String)**

```csharp
public void ExchangeBindingCreated(TContext context, string source, string destination, string routingKey)
```

#### Parameters

`context` TContext<br/>

`source` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`destination` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **QueueDeclared(TContext, String)**

```csharp
public void QueueDeclared(TContext context, string name)
```

#### Parameters

`context` TContext<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **QueueBindingCreated(TContext, String, String)**

```csharp
public void QueueBindingCreated(TContext context, string source, string destination)
```

#### Parameters

`context` TContext<br/>

`source` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`destination` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConsumerConnected(TContext, TopologyHandle, String)**

```csharp
public TopologyHandle ConsumerConnected(TContext context, TopologyHandle handle, string queueName)
```

#### Parameters

`context` TContext<br/>

`handle` [TopologyHandle](../masstransit-transports-fabric/topologyhandle)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[TopologyHandle](../masstransit-transports-fabric/topologyhandle)<br/>
