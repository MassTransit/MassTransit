---

title: ReceiveEndpointHandle

---

# ReceiveEndpointHandle

Namespace: MassTransit

A handle to an active endpoint

```csharp
public interface ReceiveEndpointHandle
```

## Properties

### **Ready**

A task which can be awaited to know when the receive endpoint is ready

```csharp
public abstract Task<ReceiveEndpointReady> Ready { get; }
```

#### Property Value

[Task\<ReceiveEndpointReady\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Methods

### **Stop(CancellationToken)**

Stop the endpoint, releasing any resources associated with the endpoint

```csharp
Task Stop(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
