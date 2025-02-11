---

title: TopicNode<T>

---

# TopicNode\<T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public class TopicNode<T> : IProbeSite
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TopicNode\<T\>](../masstransit-transports-fabric/topicnode-1)<br/>
Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **Sinks**

```csharp
public IEnumerable<IMessageSink<T>> Sinks { get; }
```

#### Property Value

[IEnumerable\<IMessageSink\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Constructors

### **TopicNode(StringComparer)**

```csharp
public TopicNode(StringComparer comparer)
```

#### Parameters

`comparer` [StringComparer](https://learn.microsoft.com/en-us/dotnet/api/system.stringcomparer)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Add(IMessageSink\<T\>, String)**

```csharp
public ConnectHandle Add(IMessageSink<T> sink, string pattern)
```

#### Parameters

`sink` [IMessageSink\<T\>](../masstransit-transports-fabric/imessagesink-1)<br/>

`pattern` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Deliver(DeliveryContext\<T\>, String)**

```csharp
public Task Deliver(DeliveryContext<T> context, string routingKey)
```

#### Parameters

`context` [DeliveryContext\<T\>](../masstransit-transports-fabric/deliverycontext-1)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
