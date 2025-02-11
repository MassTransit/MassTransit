---

title: ConsumeContextEndpointExtensions

---

# ConsumeContextEndpointExtensions

Namespace: MassTransit

```csharp
public static class ConsumeContextEndpointExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextEndpointExtensions](../masstransit/consumecontextendpointextensions)

## Methods

### **GetFaultEndpoint\<T\>(ConsumeContext)**

Returns the endpoint for a fault, either directly to the requester, or published

```csharp
public static Task<ISendEndpoint> GetFaultEndpoint<T>(ConsumeContext context)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetFaultEndpoint\<T\>(ConsumeContext, Uri, Nullable\<Guid\>)**

Returns the endpoint for a fault, either directly to the requester, or published

```csharp
public static Task<ISendEndpoint> GetFaultEndpoint<T>(ConsumeContext context, Uri faultAddress, Nullable<Guid> requestId)
```

#### Type Parameters

`T`<br/>
The response type

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`faultAddress` Uri<br/>

`requestId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetReceiveFaultEndpoint(ReceiveContext, ConsumeContext, Nullable\<Guid\>)**

Returns the endpoint for a receive fault, either directly to the requester, or published

```csharp
public static Task<ISendEndpoint> GetReceiveFaultEndpoint(ReceiveContext context, ConsumeContext consumeContext, Nullable<Guid> requestId)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`requestId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponseEndpoint\<T\>(ConsumeContext)**

Returns the endpoint for a response, either directly to the requester, or published

```csharp
public static Task<ISendEndpoint> GetResponseEndpoint<T>(ConsumeContext context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetResponseEndpoint\<T\>(ConsumeContext, Uri, Nullable\<Guid\>)**

Returns the endpoint for a response, either directly to the requester, or published

```csharp
public static Task<ISendEndpoint> GetResponseEndpoint<T>(ConsumeContext context, Uri responseAddress, Nullable<Guid> requestId)
```

#### Type Parameters

`T`<br/>
The response type

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`responseAddress` Uri<br/>

`requestId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetPublishEndpoint\<T\>(IPublishEndpointProvider, ConsumeContext, Nullable\<Guid\>)**

```csharp
internal static Task<ISendEndpoint> GetPublishEndpoint<T>(IPublishEndpointProvider publishEndpointProvider, ConsumeContext consumeContext, Nullable<Guid> requestId)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishEndpointProvider` [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider)<br/>

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`requestId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetSendEndpoint(ISendEndpointProvider, ConsumeContext, Uri, Nullable\<Guid\>)**

```csharp
internal static Task<ISendEndpoint> GetSendEndpoint(ISendEndpointProvider sendEndpointProvider, ConsumeContext consumeContext, Uri destinationAddress, Nullable<Guid> requestId)
```

#### Parameters

`sendEndpointProvider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`requestId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
