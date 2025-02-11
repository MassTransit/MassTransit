---

title: IBusControl

---

# IBusControl

Namespace: MassTransit

```csharp
public interface IBusControl : IBus, IPublishEndpoint, IPublishObserverConnector, IPublishEndpointProvider, ISendEndpointProvider, ISendObserverConnector, IConsumePipeConnector, IRequestPipeConnector, IConsumeMessageObserverConnector, IConsumeObserverConnector, IReceiveObserverConnector, IReceiveEndpointObserverConnector, IReceiveConnector, IEndpointConfigurationObserverConnector, IProbeSite
```

Implements [IBus](../masstransit/ibus), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [IPublishEndpointProvider](../masstransit/ipublishendpointprovider), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector), [IConsumePipeConnector](../masstransit/iconsumepipeconnector), [IRequestPipeConnector](../masstransit/irequestpipeconnector), [IConsumeMessageObserverConnector](../masstransit/iconsumemessageobserverconnector), [IConsumeObserverConnector](../masstransit/iconsumeobserverconnector), [IReceiveObserverConnector](../masstransit/ireceiveobserverconnector), [IReceiveEndpointObserverConnector](../masstransit/ireceiveendpointobserverconnector), [IReceiveConnector](../masstransit/ireceiveconnector), [IEndpointConfigurationObserverConnector](../masstransit/iendpointconfigurationobserverconnector), [IProbeSite](../masstransit/iprobesite)

## Methods

### **StartAsync(CancellationToken)**

Starts the bus (assuming the battery isn't dead). Once the bus has been started it cannot be started again, even after it has been stopped.

```csharp
Task<BusHandle> StartAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<BusHandle\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The BusHandle for the started bus. This is no longer needed, as calling Stop on the IBusControl will stop the bus equally well.

### **StopAsync(CancellationToken)**

Stops the bus if it has been started. If the bus hasn't been started, the method returns without any warning.

```csharp
Task StopAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CheckHealth()**

Returns the health of the bus, including all receive endpoints

```csharp
BusHealthResult CheckHealth()
```

#### Returns

[BusHealthResult](../masstransit/bushealthresult)<br/>
