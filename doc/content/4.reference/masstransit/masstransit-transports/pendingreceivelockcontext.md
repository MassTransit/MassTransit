---

title: PendingReceiveLockContext

---

# PendingReceiveLockContext

Namespace: MassTransit.Transports

```csharp
public class PendingReceiveLockContext : ReceiveLockContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PendingReceiveLockContext](../masstransit-transports/pendingreceivelockcontext)<br/>
Implements [ReceiveLockContext](../masstransit-transports/receivelockcontext)

## Properties

### **IsEmpty**

```csharp
public bool IsEmpty { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **PendingReceiveLockContext()**

```csharp
public PendingReceiveLockContext()
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

### **Enqueue(BaseReceiveContext, ReceiveLockContext)**

```csharp
public bool Enqueue(BaseReceiveContext receiveContext, ReceiveLockContext receiveLockContext)
```

#### Parameters

`receiveContext` [BaseReceiveContext](../masstransit-transports/basereceivecontext)<br/>

`receiveLockContext` [ReceiveLockContext](../masstransit-transports/receivelockcontext)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Cancel()**

```csharp
public void Cancel()
```
