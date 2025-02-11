---

title: BaseTransactionalBus

---

# BaseTransactionalBus

Namespace: MassTransit.Transactions

```csharp
public abstract class BaseTransactionalBus : IBus, IPublishEndpoint, IPublishObserverConnector, IPublishEndpointProvider, ISendEndpointProvider, ISendObserverConnector, IConsumePipeConnector, IRequestPipeConnector, IConsumeMessageObserverConnector, IConsumeObserverConnector, IReceiveObserverConnector, IReceiveEndpointObserverConnector, IReceiveConnector, IEndpointConfigurationObserverConnector, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseTransactionalBus](../masstransit-transactions/basetransactionalbus)<br/>
Implements [IBus](../../masstransit-abstractions/masstransit/ibus), [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector), [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IReceiveConnector](../../masstransit-abstractions/masstransit/ireceiveconnector), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

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

## Methods

### **ConnectPublishObserver(IPublishObserver)**

```csharp
public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
```

#### Parameters

`observer` [IPublishObserver](../../masstransit-abstractions/masstransit/ipublishobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **GetPublishSendEndpoint\<T\>()**

```csharp
public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

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

`publishPipe` [IPipe\<PublishContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

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

`publishPipe` [IPipe\<PublishContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

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

`publishPipe` [IPipe\<PublishContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

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

`publishPipe` [IPipe\<PublishContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

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

`publishPipe` [IPipe\<PublishContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

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

`publishPipe` [IPipe\<PublishContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConnectSendObserver(ISendObserver)**

```csharp
public ConnectHandle ConnectSendObserver(ISendObserver observer)
```

#### Parameters

`observer` [ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

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

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectConsumePipe\<T\>(IPipe\<ConsumeContext\<T\>\>, ConnectPipeOptions)**

```csharp
public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`options` [ConnectPipeOptions](../../masstransit-abstractions/masstransit/connectpipeoptions)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectRequestPipe\<T\>(Guid, IPipe\<ConsumeContext\<T\>\>)**

```csharp
public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

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

### **ConnectConsumeObserver(IConsumeObserver)**

```csharp
public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
```

#### Parameters

`observer` [IConsumeObserver](../../masstransit-abstractions/masstransit/iconsumeobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectReceiveObserver(IReceiveObserver)**

```csharp
public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
```

#### Parameters

`observer` [IReceiveObserver](../../masstransit-abstractions/masstransit/ireceiveobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectReceiveEndpointObserver(IReceiveEndpointObserver)**

```csharp
public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
```

#### Parameters

`observer` [IReceiveEndpointObserver](../../masstransit-abstractions/masstransit/ireceiveendpointobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver)**

```csharp
public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
```

#### Parameters

`observer` [IEndpointConfigurationObserver](../../masstransit-abstractions/masstransit/iendpointconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

### **ConnectReceiveEndpoint(String, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Add(Func\<Task\>)**

```csharp
public abstract Task Add(Func<Task> action)
```

#### Parameters

`action` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
