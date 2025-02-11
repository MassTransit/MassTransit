---

title: ReceiveTransport<TContext>

---

# ReceiveTransport\<TContext\>

Namespace: MassTransit.Transports

```csharp
public class ReceiveTransport<TContext> : IReceiveTransport, IReceiveObserverConnector, IPublishObserverConnector, ISendObserverConnector, IReceiveTransportObserverConnector, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveTransport\<TContext\>](../masstransit-transports/receivetransport-1)<br/>
Implements [IReceiveTransport](../masstransit-transports/ireceivetransport), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IReceiveTransportObserverConnector](../../masstransit-abstractions/masstransit/ireceivetransportobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **PreStartPipe**

```csharp
public IPipe<TContext> PreStartPipe { get; set; }
```

#### Property Value

[IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Constructors

### **ReceiveTransport(IHostConfiguration, ReceiveEndpointContext, Func\<ITransportSupervisor\<TContext\>\>, IPipe\<TContext\>)**

```csharp
public ReceiveTransport(IHostConfiguration hostConfiguration, ReceiveEndpointContext context, Func<ITransportSupervisor<TContext>> supervisorFactory, IPipe<TContext> transportPipe)
```

#### Parameters

`hostConfiguration` [IHostConfiguration](../masstransit-configuration/ihostconfiguration)<br/>

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

`supervisorFactory` [Func\<ITransportSupervisor\<TContext\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`transportPipe` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Start()**

Start the receive transport, returning a Task that can be awaited to signal the transport has
 completely shutdown once the cancellation token is cancelled.

```csharp
public ReceiveTransportHandle Start()
```

#### Returns

[ReceiveTransportHandle](../masstransit-transports/receivetransporthandle)<br/>
A task that is completed once the transport is shut down

### **ConnectReceiveObserver(IReceiveObserver)**

```csharp
public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
```

#### Parameters

`observer` [IReceiveObserver](../../masstransit-abstractions/masstransit/ireceiveobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectReceiveTransportObserver(IReceiveTransportObserver)**

```csharp
public ConnectHandle ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
```

#### Parameters

`observer` [IReceiveTransportObserver](../../masstransit-abstractions/masstransit/ireceivetransportobserver)<br/>

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
