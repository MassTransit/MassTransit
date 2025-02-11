---

title: ScopedClientFactory

---

# ScopedClientFactory

Namespace: MassTransit.Clients

```csharp
public class ScopedClientFactory : IScopedClientFactory
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedClientFactory](../masstransit-clients/scopedclientfactory)<br/>
Implements [IScopedClientFactory](../masstransit/iscopedclientfactory)

## Constructors

### **ScopedClientFactory(IClientFactory, ConsumeContext)**

```csharp
public ScopedClientFactory(IClientFactory clientFactory, ConsumeContext consumeContext)
```

#### Parameters

`clientFactory` [IClientFactory](../../masstransit-abstractions/masstransit/iclientfactory)<br/>

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

## Methods

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
