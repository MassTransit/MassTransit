---

title: ISqlReceiveEndpointConfigurator

---

# ISqlReceiveEndpointConfigurator

Namespace: MassTransit

Configure a database transport receive endpoint

```csharp
public interface ISqlReceiveEndpointConfigurator : IReceiveEndpointConfigurator, IEndpointConfigurator, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IReceivePipelineConfigurator, IReceiveEndpointObserverConnector, IReceiveEndpointDependencyConnector, IReceiveEndpointDependentConnector, ISqlQueueEndpointConfigurator, ISqlQueueConfigurator
```

Implements [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator), [IEndpointConfigurator](../../masstransit-abstractions/masstransit/iendpointconfigurator), [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IReceiveEndpointDependencyConnector](../../masstransit-abstractions/masstransit/ireceiveendpointdependencyconnector), [IReceiveEndpointDependentConnector](../../masstransit-abstractions/masstransit/ireceiveendpointdependentconnector), [ISqlQueueEndpointConfigurator](../masstransit/isqlqueueendpointconfigurator), [ISqlQueueConfigurator](../masstransit/isqlqueueconfigurator)

## Properties

### **UnlockDelay**

The time to wait before the message is redelivered when faults are rethrown to the transport.
 Defaults to 0.

```csharp
public abstract Nullable<TimeSpan> UnlockDelay { set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConcurrentDeliveryLimit**

Set number of concurrent messages per PartitionKey, higher value will increase throughput but will break delivery order (default: 1).
 This applies to the concurrent receive modes only.

```csharp
public abstract int ConcurrentDeliveryLimit { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **SetReceiveMode(SqlReceiveMode, Nullable\<Int32\>)**

Set the endpoint receive mode (changes the delivery behavior of messages to use partition keys, ordering, etc.

```csharp
void SetReceiveMode(SqlReceiveMode mode, Nullable<int> concurrentDeliveryLimit)
```

#### Parameters

`mode` [SqlReceiveMode](../masstransit/sqlreceivemode)<br/>

`concurrentDeliveryLimit` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Subscribe\<T\>(Action\<ISqlTopicSubscriptionConfigurator\>)**

Adds a topic subscription to the receive endpoint by message type

```csharp
void Subscribe<T>(Action<ISqlTopicSubscriptionConfigurator> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`callback` [Action\<ISqlTopicSubscriptionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Subscribe(String, Action\<ISqlTopicSubscriptionConfigurator\>)**

Adds a topic subscription to the receive endpoint

```csharp
void Subscribe(string topicName, Action<ISqlTopicSubscriptionConfigurator> callback)
```

#### Parameters

`topicName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The topic name

`callback` [Action\<ISqlTopicSubscriptionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the topic and the subscription

### **ConfigureClient(Action\<IPipeConfigurator\<ClientContext\>\>)**

Add middleware to the receive endpoint [ClientContext](../masstransit-sqltransport/clientcontext) pipe

```csharp
void ConfigureClient(Action<IPipeConfigurator<ClientContext>> configure)
```

#### Parameters

`configure` [Action\<IPipeConfigurator\<ClientContext\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
