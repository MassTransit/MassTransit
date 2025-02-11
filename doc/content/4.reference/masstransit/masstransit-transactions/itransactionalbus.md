---

title: ITransactionalBus

---

# ITransactionalBus

Namespace: MassTransit.Transactions

```csharp
public interface ITransactionalBus : IBus, IPublishEndpoint, IPublishObserverConnector, IPublishEndpointProvider, ISendEndpointProvider, ISendObserverConnector, IConsumePipeConnector, IRequestPipeConnector, IConsumeMessageObserverConnector, IConsumeObserverConnector, IReceiveObserverConnector, IReceiveEndpointObserverConnector, IReceiveConnector, IEndpointConfigurationObserverConnector, IProbeSite
```

Implements [IBus](../../masstransit-abstractions/masstransit/ibus), [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector), [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IReceiveConnector](../../masstransit-abstractions/masstransit/ireceiveconnector), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **Release()**

```csharp
Task Release()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
