---

title: HostReceiveEndpointHandle

---

# HostReceiveEndpointHandle

Namespace: MassTransit

Returned when a receive endpoint is connected

```csharp
public interface HostReceiveEndpointHandle
```

## Properties

### **ReceiveEndpoint**

```csharp
public abstract IReceiveEndpoint ReceiveEndpoint { get; }
```

#### Property Value

[IReceiveEndpoint](../masstransit/ireceiveendpoint)<br/>

### **Ready**

Completed when the endpoint has successfully started and is ready to consume messages.

```csharp
public abstract Task<ReceiveEndpointReady> Ready { get; }
```

#### Property Value

[Task\<ReceiveEndpointReady\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Methods

### **StopAsync(CancellationToken)**

Stop the receive endpoint and remove it from the host. Once removed, the endpoint
 cannot be restarted using the [HostReceiveEndpointHandle.ReceiveEndpoint](hostreceiveendpointhandle#receiveendpoint) property directly.

```csharp
Task StopAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
Cancel the stop operation in progress

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
Completed once the receive endpoint has stopped and been removed from the host
