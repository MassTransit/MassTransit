---

title: BusInstance<TBus>

---

# BusInstance\<TBus\>

Namespace: MassTransit

When configuring multiple bus instances in a single container (MultiBus), this base class should be used
 as a the base for the additional bus instance type.

```csharp
public abstract class BusInstance<TBus> : IBusControl, IBus, IPublishEndpoint, IPublishObserverConnector, IPublishEndpointProvider, ISendEndpointProvider, ISendObserverConnector, IConsumePipeConnector, IRequestPipeConnector, IConsumeMessageObserverConnector, IConsumeObserverConnector, IReceiveObserverConnector, IReceiveEndpointObserverConnector, IReceiveConnector, IEndpointConfigurationObserverConnector, IProbeSite
```

#### Type Parameters

`TBus`<br/>
The specific bus interface type for this bus instance

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusInstance\<TBus\>](../masstransit/businstance-1)<br/>
Implements [IBusControl](../masstransit/ibuscontrol), [IBus](../masstransit/ibus), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [IPublishEndpointProvider](../masstransit/ipublishendpointprovider), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector), [IConsumePipeConnector](../masstransit/iconsumepipeconnector), [IRequestPipeConnector](../masstransit/irequestpipeconnector), [IConsumeMessageObserverConnector](../masstransit/iconsumemessageobserverconnector), [IConsumeObserverConnector](../masstransit/iconsumeobserverconnector), [IReceiveObserverConnector](../masstransit/ireceiveobserverconnector), [IReceiveEndpointObserverConnector](../masstransit/ireceiveendpointobserverconnector), [IReceiveConnector](../masstransit/ireceiveconnector), [IEndpointConfigurationObserverConnector](../masstransit/iendpointconfigurationobserverconnector), [IProbeSite](../masstransit/iprobesite)

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

[IBusTopology](../masstransit/ibustopology)<br/>

## Methods

### **ConnectPublishObserver(IPublishObserver)**

```csharp
public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
```

#### Parameters

`observer` [IPublishObserver](../masstransit/ipublishobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **Publish\<T\>(T, CancellationToken)**

```csharp
public Task Publish<T>(T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(T, IPipe\<PublishContext\<T\>\>, CancellationToken)**

```csharp
public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`publishPipe` [IPipe\<PublishContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(T, IPipe\<PublishContext\>, CancellationToken)**

```csharp
public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`publishPipe` [IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish(Object, CancellationToken)**

```csharp
public Task Publish(object message, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish(Object, IPipe\<PublishContext\>, CancellationToken)**

```csharp
public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`publishPipe` [IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish(Object, Type, CancellationToken)**

```csharp
public Task Publish(object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish(Object, Type, IPipe\<PublishContext\>, CancellationToken)**

```csharp
public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`publishPipe` [IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(Object, CancellationToken)**

```csharp
public Task Publish<T>(object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(Object, IPipe\<PublishContext\<T\>\>, CancellationToken)**

```csharp
public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`publishPipe` [IPipe\<PublishContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(Object, IPipe\<PublishContext\>, CancellationToken)**

```csharp
public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`publishPipe` [IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **GetPublishSendEndpoint\<T\>()**

```csharp
public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ConnectSendObserver(ISendObserver)**

```csharp
public ConnectHandle ConnectSendObserver(ISendObserver observer)
```

#### Parameters

`observer` [ISendObserver](../masstransit/isendobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **GetSendEndpoint(Uri)**

```csharp
public Task<ISendEndpoint> GetSendEndpoint(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ConnectConsumePipe\<T\>(IPipe\<ConsumeContext\<T\>\>)**

```csharp
public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<ConsumeContext\<T\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **ConnectConsumePipe\<T\>(IPipe\<ConsumeContext\<T\>\>, ConnectPipeOptions)**

```csharp
public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<ConsumeContext\<T\>\>](../masstransit/ipipe-1)<br/>

`options` [ConnectPipeOptions](../masstransit/connectpipeoptions)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **ConnectRequestPipe\<T\>(Guid, IPipe\<ConsumeContext\<T\>\>)**

```csharp
public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`pipe` [IPipe\<ConsumeContext\<T\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **ConnectConsumeMessageObserver\<T\>(IConsumeMessageObserver\<T\>)**

```csharp
public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
```

#### Type Parameters

`T`<br/>

#### Parameters

`observer` [IConsumeMessageObserver\<T\>](../masstransit/iconsumemessageobserver-1)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **ConnectConsumeObserver(IConsumeObserver)**

```csharp
public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
```

#### Parameters

`observer` [IConsumeObserver](../masstransit/iconsumeobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **ConnectReceiveObserver(IReceiveObserver)**

```csharp
public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
```

#### Parameters

`observer` [IReceiveObserver](../masstransit/ireceiveobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **ConnectReceiveEndpointObserver(IReceiveEndpointObserver)**

```csharp
public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
```

#### Parameters

`observer` [IReceiveEndpointObserver](../masstransit/ireceiveendpointobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver)**

```csharp
public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
```

#### Parameters

`observer` [IEndpointConfigurationObserver](../masstransit/iendpointconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **ConnectReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../masstransit/iendpointdefinition)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[HostReceiveEndpointHandle](../masstransit/hostreceiveendpointhandle)<br/>

### **ConnectReceiveEndpoint(String, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[HostReceiveEndpointHandle](../masstransit/hostreceiveendpointhandle)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../masstransit/probecontext)<br/>

### **StartAsync(CancellationToken)**

```csharp
public Task<BusHandle> StartAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<BusHandle\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **StopAsync(CancellationToken)**

```csharp
public Task StopAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CheckHealth()**

```csharp
public BusHealthResult CheckHealth()
```

#### Returns

[BusHealthResult](../masstransit/bushealthresult)<br/>
