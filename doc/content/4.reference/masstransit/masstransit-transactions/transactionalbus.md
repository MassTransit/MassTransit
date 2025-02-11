---

title: TransactionalBus

---

# TransactionalBus

Namespace: MassTransit.Transactions

```csharp
public class TransactionalBus : BaseTransactionalBus, IBus, IPublishEndpoint, IPublishObserverConnector, IPublishEndpointProvider, ISendEndpointProvider, ISendObserverConnector, IConsumePipeConnector, IRequestPipeConnector, IConsumeMessageObserverConnector, IConsumeObserverConnector, IReceiveObserverConnector, IReceiveEndpointObserverConnector, IReceiveConnector, IEndpointConfigurationObserverConnector, IProbeSite, ITransactionalBus
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseTransactionalBus](../masstransit-transactions/basetransactionalbus) → [TransactionalBus](../masstransit-transactions/transactionalbus)<br/>
Implements [IBus](../../masstransit-abstractions/masstransit/ibus), [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector), [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IReceiveConnector](../../masstransit-abstractions/masstransit/ireceiveconnector), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [ITransactionalBus](../masstransit-transactions/itransactionalbus)

## Properties

### **Address**

```csharp
public Uri Address { get; }
```

#### Property Value

Uri<br/>

### **Topology**

```csharp
public IBusTopology Topology { get; }
```

#### Property Value

[IBusTopology](../../masstransit-abstractions/masstransit/ibustopology)<br/>

## Constructors

### **TransactionalBus(IBus)**

```csharp
public TransactionalBus(IBus bus)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

## Methods

### **Release()**

```csharp
public Task Release()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Add(Func\<Task\>)**

```csharp
public Task Add(Func<Task> action)
```

#### Parameters

`action` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
