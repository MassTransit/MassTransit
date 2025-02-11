---

title: RiderHandle

---

# RiderHandle

Namespace: MassTransit.Transports

```csharp
public interface RiderHandle
```

## Properties

### **Ready**

```csharp
public abstract Task Ready { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

## Methods

### **StopAsync(CancellationToken)**

```csharp
Task StopAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
