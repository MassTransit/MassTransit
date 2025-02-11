---

title: FaultHandlerConnectHandle

---

# FaultHandlerConnectHandle

Namespace: MassTransit.Clients

The fault handler for the request client

```csharp
public class FaultHandlerConnectHandle : HandlerConnectHandle, ConnectHandle, IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultHandlerConnectHandle](../masstransit-clients/faulthandlerconnecthandle)<br/>
Implements [HandlerConnectHandle](../masstransit-clients/handlerconnecthandle), [ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Constructors

### **FaultHandlerConnectHandle(ConnectHandle)**

```csharp
public FaultHandlerConnectHandle(ConnectHandle handle)
```

#### Parameters

`handle` [ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

## Methods

### **Dispose()**

```csharp
public void Dispose()
```

### **Disconnect()**

```csharp
public void Disconnect()
```

### **TrySetException(Exception)**

```csharp
public void TrySetException(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **TrySetCanceled(CancellationToken)**

```csharp
public void TrySetCanceled(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
