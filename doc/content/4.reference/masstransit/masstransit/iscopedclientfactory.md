---

title: IScopedClientFactory

---

# IScopedClientFactory

Namespace: MassTransit

A scoped client factory

```csharp
public interface IScopedClientFactory
```

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

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../../masstransit-abstractions/masstransit/requesthandle-1)<br/>

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

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../../masstransit-abstractions/masstransit/requesthandle-1)<br/>

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

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../../masstransit-abstractions/masstransit/requesthandle-1)<br/>

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

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[RequestHandle\<T\>](../../masstransit-abstractions/masstransit/requesthandle-1)<br/>

### **CreateRequestClient\<T\>(RequestTimeout)**

Create a request client for the specified message type

```csharp
IRequestClient<T> CreateRequestClient<T>(RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

#### Returns

[IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

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

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
The default timeout for requests

#### Returns

[IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>
