---

title: IReceiveEndpointCollection

---

# IReceiveEndpointCollection

Namespace: MassTransit.Transports

```csharp
public interface IReceiveEndpointCollection : IReceiveEndpointObserverConnector, IConsumeMessageObserverConnector, IProbeSite
```

Implements [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **Add(String, ReceiveEndpoint)**

Add an endpoint to the collection

```csharp
void Add(string endpointName, ReceiveEndpoint endpoint)
```

#### Parameters

`endpointName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`endpoint` [ReceiveEndpoint](../masstransit-transports/receiveendpoint)<br/>

### **StartEndpoints(CancellationToken)**

Start all endpoints in the collection which have not been started, and return the handles
 for those endpoints.

```csharp
HostReceiveEndpointHandle[] StartEndpoints(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[HostReceiveEndpointHandle[]](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

### **Start(String, CancellationToken)**

Start a new receive endpoint

```csharp
HostReceiveEndpointHandle Start(string endpointName, CancellationToken cancellationToken)
```

#### Parameters

`endpointName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

### **StopEndpoints(CancellationToken)**

Stop all receive endpoints

```csharp
Task StopEndpoints(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CheckEndpointHealth()**

```csharp
IEnumerable<EndpointHealthResult> CheckEndpointHealth()
```

#### Returns

[IEnumerable\<EndpointHealthResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
