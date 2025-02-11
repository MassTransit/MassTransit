---

title: ReceiveEndpointDispatcher

---

# ReceiveEndpointDispatcher

Namespace: MassTransit.Transports

```csharp
public class ReceiveEndpointDispatcher : IReceiveEndpointDispatcher, IConsumeObserverConnector, IConsumeMessageObserverConnector, IDispatchMetrics, IReceiveObserverConnector, IPublishObserverConnector, ISendObserverConnector, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveEndpointDispatcher](../masstransit-transports/receiveendpointdispatcher)<br/>
Implements [IReceiveEndpointDispatcher](../masstransit-transports/ireceiveendpointdispatcher), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IDispatchMetrics](../masstransit-transports/idispatchmetrics), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **ActiveDispatchCount**

```csharp
public int ActiveDispatchCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **DispatchCount**

```csharp
public long DispatchCount { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **MaxConcurrentDispatchCount**

```csharp
public int MaxConcurrentDispatchCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **InputAddress**

```csharp
public Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

## Constructors

### **ReceiveEndpointDispatcher(ReceiveEndpointContext)**

```csharp
public ReceiveEndpointDispatcher(ReceiveEndpointContext context)
```

#### Parameters

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **ConnectReceiveObserver(IReceiveObserver)**

```csharp
public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
```

#### Parameters

`observer` [IReceiveObserver](../../masstransit-abstractions/masstransit/ireceiveobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectPublishObserver(IPublishObserver)**

```csharp
public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
```

#### Parameters

`observer` [IPublishObserver](../../masstransit-abstractions/masstransit/ipublishobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectSendObserver(ISendObserver)**

```csharp
public ConnectHandle ConnectSendObserver(ISendObserver observer)
```

#### Parameters

`observer` [ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectConsumeMessageObserver\<T\>(IConsumeMessageObserver\<T\>)**

```csharp
public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
```

#### Type Parameters

`T`<br/>

#### Parameters

`observer` [IConsumeMessageObserver\<T\>](../../masstransit-abstractions/masstransit/iconsumemessageobserver-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectConsumeObserver(IConsumeObserver)**

```csharp
public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
```

#### Parameters

`observer` [IConsumeObserver](../../masstransit-abstractions/masstransit/iconsumeobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **GetMetrics()**

```csharp
public DeliveryMetrics GetMetrics()
```

#### Returns

[DeliveryMetrics](../masstransit-transports/deliverymetrics)<br/>

### **Dispatch(Byte[], IReadOnlyDictionary\<String, Object\>, CancellationToken, Object[])**

```csharp
public Task Dispatch(Byte[] body, IReadOnlyDictionary<string, object> headers, CancellationToken cancellationToken, Object[] payloads)
```

#### Parameters

`body` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`headers` [IReadOnlyDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`payloads` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

## Events

### **ZeroActivity**

```csharp
public event ZeroActiveDispatchHandler ZeroActivity;
```
