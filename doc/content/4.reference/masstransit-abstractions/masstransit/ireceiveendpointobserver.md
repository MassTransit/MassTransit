---

title: IReceiveEndpointObserver

---

# IReceiveEndpointObserver

Namespace: MassTransit

Used to observe the events signaled by a receive endpoint

```csharp
public interface IReceiveEndpointObserver
```

## Methods

### **Ready(ReceiveEndpointReady)**

Called when the receive endpoint is ready to receive messages

```csharp
Task Ready(ReceiveEndpointReady ready)
```

#### Parameters

`ready` [ReceiveEndpointReady](../masstransit/receiveendpointready)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stopping(ReceiveEndpointStopping)**

Called when the receive endpoint is being stopped, prior to actually stopping

```csharp
Task Stopping(ReceiveEndpointStopping stopping)
```

#### Parameters

`stopping` [ReceiveEndpointStopping](../masstransit/receiveendpointstopping)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Completed(ReceiveEndpointCompleted)**

Called when the receive endpoint has completed

```csharp
Task Completed(ReceiveEndpointCompleted completed)
```

#### Parameters

`completed` [ReceiveEndpointCompleted](../masstransit/receiveendpointcompleted)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted(ReceiveEndpointFaulted)**

Called when the receive endpoint faults

```csharp
Task Faulted(ReceiveEndpointFaulted faulted)
```

#### Parameters

`faulted` [ReceiveEndpointFaulted](../masstransit/receiveendpointfaulted)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
