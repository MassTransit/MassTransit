---

title: MessageQueue<TContext, T>

---

# MessageQueue\<TContext, T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public class MessageQueue<TContext, T> : Agent, IAgent, IMessageQueue<TContext, T>, IMessageSink<T>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../../masstransit-abstractions/masstransit-middleware/agent) → [MessageQueue\<TContext, T\>](../masstransit-transports-fabric/messagequeue-2)<br/>
Implements [IAgent](../../masstransit-abstractions/masstransit/iagent), [IMessageQueue\<TContext, T\>](../masstransit-transports-fabric/imessagequeue-2), [IMessageSink\<T\>](../masstransit-transports-fabric/imessagesink-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **Name**

```csharp
public string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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

### **MessageQueue(IMessageFabricObserver\<TContext\>, String)**

```csharp
public MessageQueue(IMessageFabricObserver<TContext> observer, string name)
```

#### Parameters

`observer` [IMessageFabricObserver\<TContext\>](../masstransit-transports-fabric/imessagefabricobserver-1)<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **ConnectMessageReceiver(TContext, IMessageReceiver\<T\>)**

```csharp
public TopologyHandle ConnectMessageReceiver(TContext nodeContext, IMessageReceiver<T> receiver)
```

#### Parameters

`nodeContext` TContext<br/>

`receiver` [IMessageReceiver\<T\>](../masstransit-transports-fabric/imessagereceiver-1)<br/>

#### Returns

[TopologyHandle](../masstransit-transports-fabric/topologyhandle)<br/>

### **Deliver(DeliveryContext\<T\>)**

```csharp
public Task Deliver(DeliveryContext<T> context)
```

#### Parameters

`context` [DeliveryContext\<T\>](../masstransit-transports-fabric/deliverycontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **StopAgent(StopContext)**

```csharp
protected Task StopAgent(StopContext context)
```

#### Parameters

`context` [StopContext](../../masstransit-abstractions/masstransit/stopcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
