---

title: ClientFactoryContext

---

# ClientFactoryContext

Namespace: MassTransit

The client factory context, which contains multiple interfaces and properties used by clients

```csharp
public interface ClientFactoryContext : IConsumePipeConnector, IRequestPipeConnector
```

Implements [IConsumePipeConnector](../masstransit/iconsumepipeconnector), [IRequestPipeConnector](../masstransit/irequestpipeconnector)

## Properties

### **DefaultTimeout**

Default timeout for requests

```csharp
public abstract RequestTimeout DefaultTimeout { get; }
```

#### Property Value

[RequestTimeout](../masstransit/requesttimeout)<br/>

### **ResponseAddress**

The address used for responses to messages sent by this client

```csharp
public abstract Uri ResponseAddress { get; }
```

#### Property Value

Uri<br/>

## Methods

### **GetRequestEndpoint\<T\>(ConsumeContext)**

Returns an endpoint to which requests are sent

```csharp
IRequestSendEndpoint<T> GetRequestEndpoint<T>(ConsumeContext consumeContext)
```

#### Type Parameters

`T`<br/>

#### Parameters

`consumeContext` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[IRequestSendEndpoint\<T\>](../masstransit/irequestsendendpoint-1)<br/>

### **GetRequestEndpoint\<T\>(Uri, ConsumeContext)**

Returns an endpoint to which requests are sent

```csharp
IRequestSendEndpoint<T> GetRequestEndpoint<T>(Uri destinationAddress, ConsumeContext consumeContext)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`consumeContext` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[IRequestSendEndpoint\<T\>](../masstransit/irequestsendendpoint-1)<br/>
