---

title: ClientFactoryExtensions

---

# ClientFactoryExtensions

Namespace: MassTransit

```csharp
public static class ClientFactoryExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ClientFactoryExtensions](../masstransit/clientfactoryextensions)

## Methods

### **CreateRequestClient\<TRequest\>(IBus, Uri, RequestTimeout)**

Create a request client from the bus, using the default bus endpoint for responses

```csharp
public static IRequestClient<TRequest> CreateRequestClient<TRequest>(IBus bus, Uri destinationAddress, RequestTimeout timeout)
```

#### Type Parameters

`TRequest`<br/>
The request type

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
The bus instance

`destinationAddress` Uri<br/>
The request service address

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
The default request timeout

#### Returns

[IRequestClient\<TRequest\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

### **CreateRequestClient\<TRequest\>(IBus, RequestTimeout)**

Create a request client from the bus, using the default bus endpoint for responses, and publishing the request versus sending it.

```csharp
public static IRequestClient<TRequest> CreateRequestClient<TRequest>(IBus bus, RequestTimeout timeout)
```

#### Type Parameters

`TRequest`<br/>
The request type

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
The bus instance

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
The default request timeout

#### Returns

[IRequestClient\<TRequest\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

### **CreateRequestClient\<TRequest\>(ConsumeContext, IBus, Uri, RequestTimeout)**

Create a request client from the bus, using the default bus endpoint for responses

```csharp
public static IRequestClient<TRequest> CreateRequestClient<TRequest>(ConsumeContext consumeContext, IBus bus, Uri destinationAddress, RequestTimeout timeout)
```

#### Type Parameters

`TRequest`<br/>
The request type

#### Parameters

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
The bus instance

`destinationAddress` Uri<br/>
The request service address

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
The default request timeout

#### Returns

[IRequestClient\<TRequest\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

### **CreateRequestClient\<TRequest\>(ConsumeContext, IBus, RequestTimeout)**

Create a request client from the bus, using the default bus endpoint for responses

```csharp
public static IRequestClient<TRequest> CreateRequestClient<TRequest>(ConsumeContext consumeContext, IBus bus, RequestTimeout timeout)
```

#### Type Parameters

`TRequest`<br/>
The request type

#### Parameters

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
The bus instance

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
The default request timeout

#### Returns

[IRequestClient\<TRequest\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

### **CreateClientFactory(IBus, RequestTimeout)**

Create a client factory from the bus, which uses the default bus endpoint for any response messages

```csharp
public static IClientFactory CreateClientFactory(IBus bus, RequestTimeout timeout)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
THe bus instance

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
The default request timeout

#### Returns

[IClientFactory](../../masstransit-abstractions/masstransit/iclientfactory)<br/>

### **CreateClientFactory(HostReceiveEndpointHandle, RequestTimeout)**

Connects a client factory to a host receive endpoint, using the bus as the send endpoint provider

```csharp
public static IClientFactory CreateClientFactory(HostReceiveEndpointHandle receiveEndpointHandle, RequestTimeout timeout)
```

#### Parameters

`receiveEndpointHandle` [HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>
A handle to the receive endpoint, which is stopped when the client factory is disposed

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[IClientFactory](../../masstransit-abstractions/masstransit/iclientfactory)<br/>

### **CreateClientFactory(IReceiveConnector, RequestTimeout)**

Connects a new receive endpoint to the host, and creates a .

```csharp
public static IClientFactory CreateClientFactory(IReceiveConnector connector, RequestTimeout timeout)
```

#### Parameters

`connector` [IReceiveConnector](../../masstransit-abstractions/masstransit/ireceiveconnector)<br/>
The host to connect the new receive endpoint

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
The default request timeout

#### Returns

[IClientFactory](../../masstransit-abstractions/masstransit/iclientfactory)<br/>

### **ConnectClientFactory(IReceiveConnector, RequestTimeout)**

Connects a new receive endpoint to the host, and creates a .

```csharp
public static IClientFactory ConnectClientFactory(IReceiveConnector connector, RequestTimeout timeout)
```

#### Parameters

`connector` [IReceiveConnector](../../masstransit-abstractions/masstransit/ireceiveconnector)<br/>
The host to connect the new receive endpoint

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
The default request timeout

#### Returns

[IClientFactory](../../masstransit-abstractions/masstransit/iclientfactory)<br/>
