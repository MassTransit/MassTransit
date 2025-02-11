---

title: ClientFactory

---

# ClientFactory

Namespace: MassTransit.Clients

```csharp
public class ClientFactory : IClientFactory, IAsyncDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ClientFactory](../masstransit-clients/clientfactory)<br/>
Implements [IClientFactory](../../masstransit-abstractions/masstransit/iclientfactory), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public ClientFactoryContext Context { get; }
```

#### Property Value

[ClientFactoryContext](../../masstransit-abstractions/masstransit/clientfactorycontext)<br/>

## Constructors

### **ClientFactory(ClientFactoryContext)**

```csharp
public ClientFactory(ClientFactoryContext context)
```

#### Parameters

`context` [ClientFactoryContext](../../masstransit-abstractions/masstransit/clientfactorycontext)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>

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
