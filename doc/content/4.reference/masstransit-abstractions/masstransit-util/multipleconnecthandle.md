---

title: MultipleConnectHandle

---

# MultipleConnectHandle

Namespace: MassTransit.Util

```csharp
public class MultipleConnectHandle : ConnectHandle, IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MultipleConnectHandle](../masstransit-util/multipleconnecthandle)<br/>
Implements [ConnectHandle](../masstransit/connecthandle), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Constructors

### **MultipleConnectHandle(IEnumerable\<ConnectHandle\>)**

```csharp
public MultipleConnectHandle(IEnumerable<ConnectHandle> handles)
```

#### Parameters

`handles` [IEnumerable\<ConnectHandle\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **MultipleConnectHandle(ConnectHandle[])**

```csharp
public MultipleConnectHandle(ConnectHandle[] handles)
```

#### Parameters

`handles` [ConnectHandle[]](../masstransit/connecthandle)<br/>

## Methods

### **Disconnect()**

```csharp
public void Disconnect()
```

### **Dispose()**

```csharp
public void Dispose()
```
