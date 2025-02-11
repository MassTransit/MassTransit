---

title: MessageTopicExchange<T>

---

# MessageTopicExchange\<T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public class MessageTopicExchange<T> : IMessageExchange<T>, IMessageSink<T>, IProbeSite, IMessageSource<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageTopicExchange\<T\>](../masstransit-transports-fabric/messagetopicexchange-1)<br/>
Implements [IMessageExchange\<T\>](../masstransit-transports-fabric/imessageexchange-1), [IMessageSink\<T\>](../masstransit-transports-fabric/imessagesink-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IMessageSource\<T\>](../masstransit-transports-fabric/imessagesource-1)

## Properties

### **Sinks**

```csharp
public IEnumerable<IMessageSink<T>> Sinks { get; }
```

#### Property Value

[IEnumerable\<IMessageSink\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Name**

```csharp
public string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **MessageTopicExchange(String, StringComparer)**

```csharp
public MessageTopicExchange(string name, StringComparer comparer)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`comparer` [StringComparer](https://learn.microsoft.com/en-us/dotnet/api/system.stringcomparer)<br/>

## Methods

### **Deliver(DeliveryContext\<T\>)**

```csharp
public Task Deliver(DeliveryContext<T> context)
```

#### Parameters

`context` [DeliveryContext\<T\>](../masstransit-transports-fabric/deliverycontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Connect(IMessageSink\<T\>, String)**

```csharp
public ConnectHandle Connect(IMessageSink<T> sink, string routingKey)
```

#### Parameters

`sink` [IMessageSink\<T\>](../masstransit-transports-fabric/imessagesink-1)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
