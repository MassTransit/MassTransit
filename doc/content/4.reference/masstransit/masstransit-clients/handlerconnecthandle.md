---

title: HandlerConnectHandle

---

# HandlerConnectHandle

Namespace: MassTransit.Clients

```csharp
public interface HandlerConnectHandle : ConnectHandle, IDisposable
```

Implements [ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Methods

### **TrySetException(Exception)**

```csharp
void TrySetException(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **TrySetCanceled(CancellationToken)**

```csharp
void TrySetCanceled(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
