---

title: ReceiveLockContext

---

# ReceiveLockContext

Namespace: MassTransit.Transports

Encapsulates a transport lock

```csharp
public interface ReceiveLockContext
```

## Methods

### **Complete()**

Called to complete the message

```csharp
Task Complete()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted(Exception)**

Called if the message was faulted. This method should NOT throw an exception.

```csharp
Task Faulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ValidateLockStatus()**

Validate that the lock is still valid

```csharp
Task ValidateLockStatus()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
