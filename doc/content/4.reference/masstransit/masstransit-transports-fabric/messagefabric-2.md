---

title: MessageFabric<TContext, T>

---

# MessageFabric\<TContext, T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public class MessageFabric<TContext, T> : Supervisor, IAgent, ISupervisor, IMessageFabric<TContext, T>, IMessageFabricObserverConnector<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../../masstransit-abstractions/masstransit-middleware/agent) → [Supervisor](../../masstransit-abstractions/masstransit-middleware/supervisor) → [MessageFabric\<TContext, T\>](../masstransit-transports-fabric/messagefabric-2)<br/>
Implements [IAgent](../../masstransit-abstractions/masstransit/iagent), [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor), [IMessageFabric\<TContext, T\>](../masstransit-transports-fabric/imessagefabric-2), [IMessageFabricObserverConnector\<TContext\>](../masstransit-transports-fabric/imessagefabricobserverconnector-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **PeakActiveCount**

```csharp
public int PeakActiveCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **TotalCount**

```csharp
public long TotalCount { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Ready**

```csharp
public Task Ready { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Completed**

```csharp
public Task Completed { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stopping**

```csharp
public CancellationToken Stopping { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Stopped**

```csharp
public CancellationToken Stopped { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Constructors

### **MessageFabric()**

```csharp
public MessageFabric()
```

## Methods

### **ExchangeDeclare(TContext, String, ExchangeType)**

```csharp
public void ExchangeDeclare(TContext context, string name, ExchangeType exchangeType)
```

#### Parameters

`context` TContext<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

### **ExchangeBind(TContext, String, String, String)**

```csharp
public void ExchangeBind(TContext context, string source, string destination, string routingKey)
```

#### Parameters

`context` TContext<br/>

`source` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`destination` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **QueueDeclare(TContext, String)**

```csharp
public void QueueDeclare(TContext context, string name)
```

#### Parameters

`context` TContext<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **QueueBind(TContext, String, String)**

```csharp
public void QueueBind(TContext context, string source, string destination)
```

#### Parameters

`context` TContext<br/>

`source` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`destination` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **GetExchange(TContext, String, ExchangeType)**

```csharp
public IMessageExchange<T> GetExchange(TContext context, string name, ExchangeType exchangeType)
```

#### Parameters

`context` TContext<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

#### Returns

[IMessageExchange\<T\>](../masstransit-transports-fabric/imessageexchange-1)<br/>

### **GetQueue(TContext, String)**

```csharp
public IMessageQueue<TContext, T> GetQueue(TContext context, string name)
```

#### Parameters

`context` TContext<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IMessageQueue\<TContext, T\>](../masstransit-transports-fabric/imessagequeue-2)<br/>

### **ConnectMessageFabricObserver(IMessageFabricObserver\<TContext\>)**

```csharp
public ConnectHandle ConnectMessageFabricObserver(IMessageFabricObserver<TContext> observer)
```

#### Parameters

`observer` [IMessageFabricObserver\<TContext\>](../masstransit-transports-fabric/imessagefabricobserver-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
