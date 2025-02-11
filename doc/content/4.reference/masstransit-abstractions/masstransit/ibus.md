---

title: IBus

---

# IBus

Namespace: MassTransit

A bus is a logical element that includes a local endpoint and zero or more receive endpoints

```csharp
public interface IBus : IPublishEndpoint, IPublishObserverConnector, IPublishEndpointProvider, ISendEndpointProvider, ISendObserverConnector, IConsumePipeConnector, IRequestPipeConnector, IConsumeMessageObserverConnector, IConsumeObserverConnector, IReceiveObserverConnector, IReceiveEndpointObserverConnector, IReceiveConnector, IEndpointConfigurationObserverConnector, IProbeSite
```

Implements [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [IPublishEndpointProvider](../masstransit/ipublishendpointprovider), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector), [IConsumePipeConnector](../masstransit/iconsumepipeconnector), [IRequestPipeConnector](../masstransit/irequestpipeconnector), [IConsumeMessageObserverConnector](../masstransit/iconsumemessageobserverconnector), [IConsumeObserverConnector](../masstransit/iconsumeobserverconnector), [IReceiveObserverConnector](../masstransit/ireceiveobserverconnector), [IReceiveEndpointObserverConnector](../masstransit/ireceiveendpointobserverconnector), [IReceiveConnector](../masstransit/ireceiveconnector), [IEndpointConfigurationObserverConnector](../masstransit/iendpointconfigurationobserverconnector), [IProbeSite](../masstransit/iprobesite)

## Properties

### **Address**

The InputAddress of the default bus endpoint

```csharp
public abstract Uri Address { get; }
```

#### Property Value

Uri<br/>

### **Topology**

The bus topology

```csharp
public abstract IBusTopology Topology { get; }
```

#### Property Value

[IBusTopology](../masstransit/ibustopology)<br/>
