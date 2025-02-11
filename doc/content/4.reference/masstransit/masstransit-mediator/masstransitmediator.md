---

title: MassTransitMediator

---

# MassTransitMediator

Namespace: MassTransit.Mediator

Sends messages directly to the , without serialization

```csharp
public class MassTransitMediator : IMediator, ISendEndpoint, ISendObserverConnector, IPublishEndpoint, IPublishObserverConnector, IPublishEndpointProvider, IClientFactory, IConsumePipeConnector, IRequestPipeConnector, IConsumeObserverConnector, IConsumeMessageObserverConnector, IAsyncDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MassTransitMediator](../masstransit-mediator/masstransitmediator)<br/>
Implements [IMediator](../../masstransit-abstractions/masstransit-mediator/imediator), [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider), [IClientFactory](../../masstransit-abstractions/masstransit/iclientfactory), [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector), [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public ClientFactoryContext Context { get; }
```

#### Property Value

[ClientFactoryContext](../../masstransit-abstractions/masstransit/clientfactorycontext)<br/>

## Constructors

### **MassTransitMediator(ILogContext, IReceiveEndpointConfiguration, IReceivePipeDispatcher, IReceiveEndpointConfiguration, IReceivePipeDispatcher)**

```csharp
public MassTransitMediator(ILogContext logContext, IReceiveEndpointConfiguration configuration, IReceivePipeDispatcher dispatcher, IReceiveEndpointConfiguration responseConfiguration, IReceivePipeDispatcher responseDispatcher)
```

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`configuration` [IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration)<br/>

`dispatcher` [IReceivePipeDispatcher](../masstransit-transports/ireceivepipedispatcher)<br/>

`responseConfiguration` [IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration)<br/>

`responseDispatcher` [IReceivePipeDispatcher](../masstransit-transports/ireceivepipedispatcher)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>

### **ConnectSendObserver(ISendObserver)**

```csharp
public ConnectHandle ConnectSendObserver(ISendObserver observer)
```

#### Parameters

`observer` [ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Send\<T\>(T, CancellationToken)**

```csharp
public Task Send<T>(T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send\<T\>(T, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send\<T\>(T, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send(Object, CancellationToken)**

```csharp
public Task Send(object message, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send(Object, Type, CancellationToken)**

```csharp
public Task Send(object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send(Object, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send(Object, Type, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send\<T\>(Object, CancellationToken)**

```csharp
public Task Send<T>(object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send\<T\>(Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send\<T\>(Object, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

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

### **CreateRequest\<T\>(T, CancellationToken, RequestTimeout)**

```csharp
public RequestHandle<T> CreateRequest<T>(T message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../../masstransit-abstractions/masstransit/requesthandle-1)<br/>

### **CreateRequest\<T\>(Uri, T, CancellationToken, RequestTimeout)**

```csharp
public RequestHandle<T> CreateRequest<T>(Uri destinationAddress, T message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../../masstransit-abstractions/masstransit/requesthandle-1)<br/>

### **CreateRequest\<T\>(ConsumeContext, T, CancellationToken, RequestTimeout)**

```csharp
public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, T message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../../masstransit-abstractions/masstransit/requesthandle-1)<br/>

### **CreateRequest\<T\>(ConsumeContext, Uri, T, CancellationToken, RequestTimeout)**

```csharp
public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, T message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../../masstransit-abstractions/masstransit/requesthandle-1)<br/>

### **CreateRequest\<T\>(Object, CancellationToken, RequestTimeout)**

```csharp
public RequestHandle<T> CreateRequest<T>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../../masstransit-abstractions/masstransit/requesthandle-1)<br/>

### **CreateRequest\<T\>(Uri, Object, CancellationToken, RequestTimeout)**

```csharp
public RequestHandle<T> CreateRequest<T>(Uri destinationAddress, object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../../masstransit-abstractions/masstransit/requesthandle-1)<br/>

### **CreateRequest\<T\>(ConsumeContext, Object, CancellationToken, RequestTimeout)**

```csharp
public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../../masstransit-abstractions/masstransit/requesthandle-1)<br/>

### **CreateRequest\<T\>(ConsumeContext, Uri, Object, CancellationToken, RequestTimeout)**

```csharp
public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../../masstransit-abstractions/masstransit/requesthandle-1)<br/>

### **CreateRequestClient\<T\>(RequestTimeout)**

```csharp
public IRequestClient<T> CreateRequestClient<T>(RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

### **CreateRequestClient\<T\>(ConsumeContext, RequestTimeout)**

```csharp
public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

### **CreateRequestClient\<T\>(Uri, RequestTimeout)**

```csharp
public IRequestClient<T> CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

### **CreateRequestClient\<T\>(ConsumeContext, Uri, RequestTimeout)**

```csharp
public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, Uri destinationAddress, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

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

### **ConnectConsumeObserver(IConsumeObserver)**

```csharp
public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
```

#### Parameters

`observer` [IConsumeObserver](../../masstransit-abstractions/masstransit/iconsumeobserver)<br/>

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
