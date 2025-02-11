---

title: IHost

---

# IHost

Namespace: MassTransit.Transports

```csharp
public interface IHost : IReceiveConnector, IEndpointConfigurationObserverConnector, IConsumeMessageObserverConnector, IConsumeObserverConnector, IReceiveObserverConnector, IPublishObserverConnector, ISendObserverConnector, IReceiveEndpointObserverConnector, IProbeSite
```

Implements [IReceiveConnector](../../masstransit-abstractions/masstransit/ireceiveconnector), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **Address**

```csharp
public abstract Uri Address { get; }
```

#### Property Value

Uri<br/>

### **Topology**

```csharp
public abstract IBusTopology Topology { get; }
```

#### Property Value

[IBusTopology](../../masstransit-abstractions/masstransit/ibustopology)<br/>

## Methods

### **Start(CancellationToken)**

```csharp
HostHandle Start(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[HostHandle](../masstransit-transports/hosthandle)<br/>

### **AddReceiveEndpoint(String, ReceiveEndpoint)**

```csharp
void AddReceiveEndpoint(string endpointName, ReceiveEndpoint receiveEndpoint)
```

#### Parameters

`endpointName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`receiveEndpoint` [ReceiveEndpoint](../masstransit-transports/receiveendpoint)<br/>

### **GetRider(String)**

```csharp
IRider GetRider(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IRider](../../masstransit-abstractions/masstransit-transports/irider)<br/>

### **AddRider(String, IRiderControl)**

```csharp
void AddRider(string name, IRiderControl riderControl)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`riderControl` [IRiderControl](../../masstransit-abstractions/masstransit-transports/iridercontrol)<br/>

### **CheckHealth(BusState, String)**

```csharp
BusHealthResult CheckHealth(BusState busState, string healthMessage)
```

#### Parameters

`busState` [BusState](../masstransit-transports/busstate)<br/>

`healthMessage` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[BusHealthResult](../../masstransit-abstractions/masstransit/bushealthresult)<br/>
