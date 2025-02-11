---

title: ReceiveEndpointCollection

---

# ReceiveEndpointCollection

Namespace: MassTransit.Transports

```csharp
public class ReceiveEndpointCollection : IReceiveEndpointCollection, IReceiveEndpointObserverConnector, IConsumeMessageObserverConnector, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveEndpointCollection](../masstransit-transports/receiveendpointcollection)<br/>
Implements [IReceiveEndpointCollection](../masstransit-transports/ireceiveendpointcollection), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ReceiveEndpointCollection()**

```csharp
public ReceiveEndpointCollection()
```

## Methods

### **Add(String, ReceiveEndpoint)**

```csharp
public void Add(string endpointName, ReceiveEndpoint endpoint)
```

#### Parameters

`endpointName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`endpoint` [ReceiveEndpoint](../masstransit-transports/receiveendpoint)<br/>

### **StartEndpoints(CancellationToken)**

```csharp
public HostReceiveEndpointHandle[] StartEndpoints(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[HostReceiveEndpointHandle[]](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

### **Start(String, CancellationToken)**

```csharp
public HostReceiveEndpointHandle Start(string endpointName, CancellationToken cancellationToken)
```

#### Parameters

`endpointName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **ConnectReceiveEndpointObserver(IReceiveEndpointObserver)**

```csharp
public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
```

#### Parameters

`observer` [IReceiveEndpointObserver](../../masstransit-abstractions/masstransit/ireceiveendpointobserver)<br/>

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

### **CheckEndpointHealth()**

```csharp
public IEnumerable<EndpointHealthResult> CheckEndpointHealth()
```

#### Returns

[IEnumerable\<EndpointHealthResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **StopEndpoints(CancellationToken)**

```csharp
public Task StopEndpoints(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
