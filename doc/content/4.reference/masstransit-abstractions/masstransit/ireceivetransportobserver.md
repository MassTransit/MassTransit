---

title: IReceiveTransportObserver

---

# IReceiveTransportObserver

Namespace: MassTransit

Used to observe the events signaled by a receive endpoint

```csharp
public interface IReceiveTransportObserver
```

## Methods

### **Ready(ReceiveTransportReady)**

Called when the receive endpoint is ready to receive messages

```csharp
Task Ready(ReceiveTransportReady ready)
```

#### Parameters

`ready` [ReceiveTransportReady](../masstransit/receivetransportready)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Completed(ReceiveTransportCompleted)**

Called when the receive endpoint has completed

```csharp
Task Completed(ReceiveTransportCompleted completed)
```

#### Parameters

`completed` [ReceiveTransportCompleted](../masstransit/receivetransportcompleted)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted(ReceiveTransportFaulted)**

Called when the receive endpoint faults

```csharp
Task Faulted(ReceiveTransportFaulted faulted)
```

#### Parameters

`faulted` [ReceiveTransportFaulted](../masstransit/receivetransportfaulted)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
