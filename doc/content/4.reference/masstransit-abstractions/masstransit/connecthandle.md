---

title: ConnectHandle

---

# ConnectHandle

Namespace: MassTransit

A connect handle is returned by a non-asynchronous resource that supports
 disconnection (such as removing an observer, etc.)

```csharp
public interface ConnectHandle : IDisposable
```

Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Methods

### **Disconnect()**

Explicitly disconnect the handle without waiting for it to be disposed. If the 
 connection is disconnected, the disconnect will be ignored when the handle is disposed.

```csharp
void Disconnect()
```
