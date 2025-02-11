---

title: ReceiveEndpointContext

---

# ReceiveEndpointContext

Namespace: MassTransit.Transports

The context of a receive endpoint

```csharp
public interface ReceiveEndpointContext : PipeContext, ISendObserverConnector, IPublishObserverConnector, IReceiveTransportObserverConnector, IReceiveObserverConnector, IReceiveEndpointObserverConnector, IProbeSite
```

Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [IReceiveTransportObserverConnector](../../masstransit-abstractions/masstransit/ireceivetransportobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **ConsumerStopTimeout**

```csharp
public abstract Nullable<TimeSpan> ConsumerStopTimeout { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StopTimeout**

```csharp
public abstract Nullable<TimeSpan> StopTimeout { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InputAddress**

```csharp
public abstract Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

### **IsBusEndpoint**

```csharp
public abstract bool IsBusEndpoint { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **EndpointObservers**

```csharp
public abstract IReceiveEndpointObserver EndpointObservers { get; }
```

#### Property Value

[IReceiveEndpointObserver](../../masstransit-abstractions/masstransit/ireceiveendpointobserver)<br/>

### **ReceiveObservers**

```csharp
public abstract IReceiveObserver ReceiveObservers { get; }
```

#### Property Value

[IReceiveObserver](../../masstransit-abstractions/masstransit/ireceiveobserver)<br/>

### **TransportObservers**

```csharp
public abstract IReceiveTransportObserver TransportObservers { get; }
```

#### Property Value

[IReceiveTransportObserver](../../masstransit-abstractions/masstransit/ireceivetransportobserver)<br/>

### **LogContext**

```csharp
public abstract ILogContext LogContext { get; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **Publish**

```csharp
public abstract IPublishTopology Publish { get; }
```

#### Property Value

[IPublishTopology](../../masstransit-abstractions/masstransit/ipublishtopology)<br/>

### **ReceivePipe**

```csharp
public abstract IReceivePipe ReceivePipe { get; }
```

#### Property Value

[IReceivePipe](../../masstransit-abstractions/masstransit-transports/ireceivepipe)<br/>

### **PublishEndpointProvider**

```csharp
public abstract IPublishEndpointProvider PublishEndpointProvider { get; }
```

#### Property Value

[IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider)<br/>

### **SendEndpointProvider**

```csharp
public abstract ISendEndpointProvider SendEndpointProvider { get; }
```

#### Property Value

[ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

### **DependenciesReady**

Task completed when dependencies are ready

```csharp
public abstract Task DependenciesReady { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **DependentsCompleted**

Task completed when dependants are completed

```csharp
public abstract Task DependentsCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishFaults**

If true (the default), faults should be published when no ResponseAddress or FaultAddress are present.

```csharp
public abstract bool PublishFaults { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PrefetchCount**

```csharp
public abstract int PrefetchCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrentMessageLimit**

```csharp
public abstract Nullable<int> ConcurrentMessageLimit { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Serialization**

```csharp
public abstract ISerialization Serialization { get; }
```

#### Property Value

[ISerialization](../../masstransit-abstractions/masstransit/iserialization)<br/>

## Methods

### **ConvertException(Exception, String)**

Convert an unknown exception to a , so that it can be used by
 the transport retry policy.

```csharp
Exception ConvertException(Exception exception, string message)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The original exception

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
A contextual message describing when the exception occurred

#### Returns

[Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **CreateReceivePipeDispatcher()**

```csharp
IReceivePipeDispatcher CreateReceivePipeDispatcher()
```

#### Returns

[IReceivePipeDispatcher](../masstransit-transports/ireceivepipedispatcher)<br/>

### **Reset()**

Reset the receive endpoint, which should clear any caches, etc.

```csharp
void Reset()
```

### **AddConsumeAgent(IAgent)**

Add an consume-side agent, which should be stopped during shutdown

```csharp
void AddConsumeAgent(IAgent agent)
```

#### Parameters

`agent` [IAgent](../../masstransit-abstractions/masstransit/iagent)<br/>

### **AddSendAgent(IAgent)**

Add an agent, which should be stopped during shutdown after consume/send agents have been stopped

```csharp
void AddSendAgent(IAgent agent)
```

#### Parameters

`agent` [IAgent](../../masstransit-abstractions/masstransit/iagent)<br/>
