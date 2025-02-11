---

title: IClientFactory

---

# IClientFactory

Namespace: MassTransit

The client factory is used to create request clients

```csharp
public interface IClientFactory
```

## Properties

### **Context**

```csharp
public abstract ClientFactoryContext Context { get; }
```

#### Property Value

[ClientFactoryContext](../masstransit/clientfactorycontext)<br/>

## Methods

### **CreateRequest\<T\>(T, CancellationToken, RequestTimeout)**

Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.

```csharp
RequestHandle<T> CreateRequest<T>(T message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../masstransit/requesthandle-1)<br/>

### **CreateRequest\<T\>(Uri, T, CancellationToken, RequestTimeout)**

Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.

```csharp
RequestHandle<T> CreateRequest<T>(Uri destinationAddress, T message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`destinationAddress` Uri<br/>
The destination service address

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../masstransit/requesthandle-1)<br/>

### **CreateRequest\<T\>(ConsumeContext, T, CancellationToken, RequestTimeout)**

Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.

```csharp
RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, T message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`consumeContext` [ConsumeContext](../masstransit/consumecontext)<br/>
The consumeContext currently being processed

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../masstransit/requesthandle-1)<br/>

### **CreateRequest\<T\>(ConsumeContext, Uri, T, CancellationToken, RequestTimeout)**

Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.

```csharp
RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, T message, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`consumeContext` [ConsumeContext](../masstransit/consumecontext)<br/>
The consumeContext currently being processed

`destinationAddress` Uri<br/>
The destination service address

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../masstransit/requesthandle-1)<br/>

### **CreateRequest\<T\>(Object, CancellationToken, RequestTimeout)**

Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.

```csharp
RequestHandle<T> CreateRequest<T>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values to initialize the message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../masstransit/requesthandle-1)<br/>

### **CreateRequest\<T\>(Uri, Object, CancellationToken, RequestTimeout)**

Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.

```csharp
RequestHandle<T> CreateRequest<T>(Uri destinationAddress, object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`destinationAddress` Uri<br/>
The destination service address

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values to initialize the message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../masstransit/requesthandle-1)<br/>

### **CreateRequest\<T\>(ConsumeContext, Object, CancellationToken, RequestTimeout)**

Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.

```csharp
RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`consumeContext` [ConsumeContext](../masstransit/consumecontext)<br/>
The consumeContext currently being processed

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values to initialize the message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../masstransit/requesthandle-1)<br/>

### **CreateRequest\<T\>(ConsumeContext, Uri, Object, CancellationToken, RequestTimeout)**

Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.

```csharp
RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, object values, CancellationToken cancellationToken, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`consumeContext` [ConsumeContext](../masstransit/consumecontext)<br/>
The consumeContext currently being processed

`destinationAddress` Uri<br/>
The destination service address

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The values to initialize the message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../masstransit/requesthandle-1)<br/>

### **CreateRequestClient\<T\>(RequestTimeout)**

Create a request client for the specified message type

```csharp
IRequestClient<T> CreateRequestClient<T>(RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>

#### Returns

[IRequestClient\<T\>](../masstransit/irequestclient-1)<br/>

### **CreateRequestClient\<T\>(ConsumeContext, RequestTimeout)**

Create a request client for the specified message type

```csharp
IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`consumeContext` [ConsumeContext](../masstransit/consumecontext)<br/>
The consumeContext currently being processed

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>

#### Returns

[IRequestClient\<T\>](../masstransit/irequestclient-1)<br/>

### **CreateRequestClient\<T\>(Uri, RequestTimeout)**

Create a request client, using the specified service address

```csharp
IRequestClient<T> CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>
The destination service address

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
The default timeout for requests

#### Returns

[IRequestClient\<T\>](../masstransit/irequestclient-1)<br/>

### **CreateRequestClient\<T\>(ConsumeContext, Uri, RequestTimeout)**

Create a request client, using the specified service address

```csharp
IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, Uri destinationAddress, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`consumeContext` [ConsumeContext](../masstransit/consumecontext)<br/>
The consumeContext currently being processed

`destinationAddress` Uri<br/>
The destination service address

`timeout` [RequestTimeout](../masstransit/requesttimeout)<br/>
The default timeout for requests

#### Returns

[IRequestClient\<T\>](../masstransit/irequestclient-1)<br/>
