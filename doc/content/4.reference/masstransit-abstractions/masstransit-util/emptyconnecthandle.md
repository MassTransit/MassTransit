---

title: EmptyConnectHandle

---

# EmptyConnectHandle

Namespace: MassTransit.Util

A do-nothing connect handle, simply to satisfy

```csharp
public class EmptyConnectHandle : ConnectHandle, IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EmptyConnectHandle](../masstransit-util/emptyconnecthandle)<br/>
Implements [ConnectHandle](../masstransit/connecthandle), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Constructors

### **EmptyConnectHandle()**

```csharp
public EmptyConnectHandle()
```

## Methods

### **Dispose()**

```csharp
public void Dispose()
```

### **Disconnect()**

```csharp
public void Disconnect()
```
