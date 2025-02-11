---

title: HostReceiveEndpointClientFactoryContext

---

# HostReceiveEndpointClientFactoryContext

Namespace: MassTransit.Clients

```csharp
public class HostReceiveEndpointClientFactoryContext : ReceiveEndpointClientFactoryContext, ClientFactoryContext, IConsumePipeConnector, IRequestPipeConnector, IAsyncDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveEndpointClientFactoryContext](../masstransit-clients/receiveendpointclientfactorycontext) → [HostReceiveEndpointClientFactoryContext](../masstransit-clients/hostreceiveendpointclientfactorycontext)<br/>
Implements [ClientFactoryContext](../../masstransit-abstractions/masstransit/clientfactorycontext), [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector), [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

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

### **HostReceiveEndpointClientFactoryContext(HostReceiveEndpointHandle, RequestTimeout)**

```csharp
public HostReceiveEndpointClientFactoryContext(HostReceiveEndpointHandle handle, RequestTimeout defaultTimeout)
```

#### Parameters

`handle` [HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

`defaultTimeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>
