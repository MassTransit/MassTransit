---

title: HostHandle

---

# HostHandle

Namespace: MassTransit.Transports

```csharp
public interface HostHandle
```

## Properties

### **Ready**

A task which can be awaited to know when the host is ready

```csharp
public abstract Task<HostReady> Ready { get; }
```

#### Property Value

[Task\<HostReady\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Methods

### **Stop(CancellationToken)**

Close the Host, shutting it down for good.

```csharp
Task Stop(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
