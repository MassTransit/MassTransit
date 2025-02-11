---

title: BusHandle

---

# BusHandle

Namespace: MassTransit

Returned once a bus has been started. Should call Stop or Dispose before the process
 can exit.

```csharp
public interface BusHandle
```

## Properties

### **Ready**

A task which can be awaited to know when the bus is ready and all of the receive endpoints have reported ready.

```csharp
public abstract Task<BusReady> Ready { get; }
```

#### Property Value

[Task\<BusReady\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Methods

### **StopAsync(CancellationToken)**

Stop the bus and all receiving endpoints on the bus. Note that cancelling the Stop
 operation may leave the bus and/or one or more receive endpoints in an indeterminate
 state.

```csharp
Task StopAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
Cancel the stop operation in progress

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
An awaitable task that is completed once everything is stopped
