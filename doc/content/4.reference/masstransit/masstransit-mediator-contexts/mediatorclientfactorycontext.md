---

title: MediatorClientFactoryContext

---

# MediatorClientFactoryContext

Namespace: MassTransit.Mediator.Contexts

```csharp
public class MediatorClientFactoryContext : ClientFactoryContext, IConsumePipeConnector, IRequestPipeConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MediatorClientFactoryContext](../masstransit-mediator-contexts/mediatorclientfactorycontext)<br/>
Implements [ClientFactoryContext](../../masstransit-abstractions/masstransit/clientfactorycontext), [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector), [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector)

## Properties

### **ResponseAddress**

```csharp
public Uri ResponseAddress { get; }
```

#### Property Value

Uri<br/>

### **DefaultTimeout**

```csharp
public RequestTimeout DefaultTimeout { get; }
```

#### Property Value

[RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

## Constructors

### **MediatorClientFactoryContext(ISendEndpoint, IConsumePipe, Uri, RequestTimeout)**

```csharp
public MediatorClientFactoryContext(ISendEndpoint endpoint, IConsumePipe connector, Uri responseAddress, RequestTimeout defaultTimeout)
```

#### Parameters

`endpoint` [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

`connector` [IConsumePipe](../../masstransit-abstractions/masstransit-transports/iconsumepipe)<br/>

`responseAddress` Uri<br/>

`defaultTimeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

## Methods

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

### **GetRequestEndpoint\<T\>(ConsumeContext)**

```csharp
public IRequestSendEndpoint<T> GetRequestEndpoint<T>(ConsumeContext consumeContext)
```

#### Type Parameters

`T`<br/>

#### Parameters

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[IRequestSendEndpoint\<T\>](../../masstransit-abstractions/masstransit/irequestsendendpoint-1)<br/>

### **GetRequestEndpoint\<T\>(Uri, ConsumeContext)**

```csharp
public IRequestSendEndpoint<T> GetRequestEndpoint<T>(Uri destinationAddress, ConsumeContext consumeContext)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[IRequestSendEndpoint\<T\>](../../masstransit-abstractions/masstransit/irequestsendendpoint-1)<br/>
