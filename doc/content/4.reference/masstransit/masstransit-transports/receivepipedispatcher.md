---

title: ReceivePipeDispatcher

---

# ReceivePipeDispatcher

Namespace: MassTransit.Transports

```csharp
public class ReceivePipeDispatcher : IReceivePipeDispatcher, IConsumePipeConnector, IConsumeObserverConnector, IConsumeMessageObserverConnector, IRequestPipeConnector, IDispatchMetrics, IReceiveObserverConnector, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceivePipeDispatcher](../masstransit-transports/receivepipedispatcher)<br/>
Implements [IReceivePipeDispatcher](../masstransit-transports/ireceivepipedispatcher), [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector), [IDispatchMetrics](../masstransit-transports/idispatchmetrics), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

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

## Constructors

### **ReceivePipeDispatcher(IReceivePipe, ReceiveObservable, IHostConfiguration, Uri)**

```csharp
public ReceivePipeDispatcher(IReceivePipe receivePipe, ReceiveObservable observers, IHostConfiguration hostConfiguration, Uri inputAddress)
```

#### Parameters

`receivePipe` [IReceivePipe](../../masstransit-abstractions/masstransit-transports/ireceivepipe)<br/>

`observers` [ReceiveObservable](../../masstransit-abstractions/masstransit-observables/receiveobservable)<br/>

`hostConfiguration` [IHostConfiguration](../masstransit-configuration/ihostconfiguration)<br/>

`inputAddress` Uri<br/>

## Methods

### **GetMetrics()**

```csharp
public DeliveryMetrics GetMetrics()
```

#### Returns

[DeliveryMetrics](../masstransit-transports/deliverymetrics)<br/>

### **Dispatch(ReceiveContext, ReceiveLockContext)**

```csharp
public Task Dispatch(ReceiveContext context, ReceiveLockContext receiveLock)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

`receiveLock` [ReceiveLockContext](../masstransit-transports/receivelockcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConnectReceiveObserver(IReceiveObserver)**

```csharp
public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
```

#### Parameters

`observer` [IReceiveObserver](../../masstransit-abstractions/masstransit/ireceiveobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **ConnectConsumePipe\<T\>(IPipe\<ConsumeContext\<T\>\>)**

```csharp
public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectConsumePipe\<T\>(IPipe\<ConsumeContext\<T\>\>, ConnectPipeOptions)**

```csharp
public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`options` [ConnectPipeOptions](../../masstransit-abstractions/masstransit/connectpipeoptions)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectRequestPipe\<T\>(Guid, IPipe\<ConsumeContext\<T\>\>)**

```csharp
public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

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

## Events

### **ZeroActivity**

```csharp
public event ZeroActiveDispatchHandler ZeroActivity;
```
