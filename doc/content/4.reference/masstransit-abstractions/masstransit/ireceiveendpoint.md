---

title: IReceiveEndpoint

---

# IReceiveEndpoint

Namespace: MassTransit

A service endpoint has an inbound transport that pushes messages to consumers

```csharp
public interface IReceiveEndpoint : ISendEndpointProvider, ISendObserverConnector, IPublishEndpointProvider, IPublishObserverConnector, IConsumePipeConnector, IRequestPipeConnector, IReceiveObserverConnector, IConsumeObserverConnector, IConsumeMessageObserverConnector, IProbeSite
```

Implements [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector), [IPublishEndpointProvider](../masstransit/ipublishendpointprovider), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [IConsumePipeConnector](../masstransit/iconsumepipeconnector), [IRequestPipeConnector](../masstransit/irequestpipeconnector), [IReceiveObserverConnector](../masstransit/ireceiveobserverconnector), [IConsumeObserverConnector](../masstransit/iconsumeobserverconnector), [IConsumeMessageObserverConnector](../masstransit/iconsumemessageobserverconnector), [IProbeSite](../masstransit/iprobesite)

## Properties

### **InputAddress**

```csharp
public abstract Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

### **Started**

```csharp
public abstract Task<ReceiveEndpointReady> Started { get; }
```

#### Property Value

[Task\<ReceiveEndpointReady\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Methods

### **Start(CancellationToken)**

Start the receive endpoint

```csharp
ReceiveEndpointHandle Start(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
Cancel the start operation in progress

#### Returns

[ReceiveEndpointHandle](../masstransit/receiveendpointhandle)<br/>
An awaitable task that is completed once everything is started

### **Stop(CancellationToken)**

Stop the receive endpoint.

```csharp
Task Stop(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
Cancel the stop operation in progress

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
An awaitable task that is completed once everything is stopped
