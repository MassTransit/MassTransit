---

title: HostRiderHandle

---

# HostRiderHandle

Namespace: MassTransit.Transports

```csharp
public interface HostRiderHandle
```

## Properties

### **Rider**

```csharp
public abstract IRider Rider { get; }
```

#### Property Value

[IRider](../masstransit-transports/irider)<br/>

### **Ready**

```csharp
public abstract Task<RiderReady> Ready { get; }
```

#### Property Value

[Task\<RiderReady\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Methods

### **StopAsync(CancellationToken)**

```csharp
Task StopAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **StopAsync(Boolean, CancellationToken)**

```csharp
Task StopAsync(bool remove, CancellationToken cancellationToken)
```

#### Parameters

`remove` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
