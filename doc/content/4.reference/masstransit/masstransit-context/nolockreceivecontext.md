---

title: NoLockReceiveContext

---

# NoLockReceiveContext

Namespace: MassTransit.Context

```csharp
public class NoLockReceiveContext : ReceiveLockContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NoLockReceiveContext](../masstransit-context/nolockreceivecontext)<br/>
Implements [ReceiveLockContext](../masstransit-transports/receivelockcontext)

## Fields

### **Instance**

```csharp
public static ReceiveLockContext Instance;
```

## Methods

### **Complete()**

```csharp
public Task Complete()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted(Exception)**

```csharp
public Task Faulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ValidateLockStatus()**

```csharp
public Task ValidateLockStatus()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
