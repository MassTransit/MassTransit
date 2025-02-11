---

title: MessageReceiverCollection<T>

---

# MessageReceiverCollection\<T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public class MessageReceiverCollection<T> : IProbeSite
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageReceiverCollection\<T\>](../masstransit-transports-fabric/messagereceivercollection-1)<br/>
Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **MessageReceiverCollection(LoadBalancerFactory\<T\>)**

```csharp
public MessageReceiverCollection(LoadBalancerFactory<T> balancerFactory)
```

#### Parameters

`balancerFactory` [LoadBalancerFactory\<T\>](../masstransit-transports-fabric/loadbalancerfactory-1)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Connect(IMessageReceiver\<T\>)**

```csharp
public TopologyHandle Connect(IMessageReceiver<T> receiver)
```

#### Parameters

`receiver` [IMessageReceiver\<T\>](../masstransit-transports-fabric/imessagereceiver-1)<br/>

#### Returns

[TopologyHandle](../masstransit-transports-fabric/topologyhandle)<br/>

### **Next(T, CancellationToken)**

```csharp
public Task<IMessageReceiver<T>> Next(T message, CancellationToken cancellationToken)
```

#### Parameters

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<IMessageReceiver\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **TryGetReceiver(Int64, IMessageReceiver\<T\>)**

```csharp
public bool TryGetReceiver(long id, out IMessageReceiver<T> consumer)
```

#### Parameters

`id` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`consumer` [IMessageReceiver\<T\>](../masstransit-transports-fabric/imessagereceiver-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
