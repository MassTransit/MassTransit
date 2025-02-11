---

title: TestReceiveEndpointObserver

---

# TestReceiveEndpointObserver

Namespace: MassTransit.Testing.Implementations

```csharp
public class TestReceiveEndpointObserver : IReceiveEndpointObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TestReceiveEndpointObserver](../masstransit-testing-implementations/testreceiveendpointobserver)<br/>
Implements [IReceiveEndpointObserver](../../masstransit-abstractions/masstransit/ireceiveendpointobserver)

## Constructors

### **TestReceiveEndpointObserver(IPublishObserver)**

```csharp
public TestReceiveEndpointObserver(IPublishObserver publishObserver)
```

#### Parameters

`publishObserver` [IPublishObserver](../../masstransit-abstractions/masstransit/ipublishobserver)<br/>

## Methods

### **Ready(ReceiveEndpointReady)**

```csharp
public Task Ready(ReceiveEndpointReady ready)
```

#### Parameters

`ready` [ReceiveEndpointReady](../../masstransit-abstractions/masstransit/receiveendpointready)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stopping(ReceiveEndpointStopping)**

```csharp
public Task Stopping(ReceiveEndpointStopping stopping)
```

#### Parameters

`stopping` [ReceiveEndpointStopping](../../masstransit-abstractions/masstransit/receiveendpointstopping)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Completed(ReceiveEndpointCompleted)**

```csharp
public Task Completed(ReceiveEndpointCompleted completed)
```

#### Parameters

`completed` [ReceiveEndpointCompleted](../../masstransit-abstractions/masstransit/receiveendpointcompleted)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted(ReceiveEndpointFaulted)**

```csharp
public Task Faulted(ReceiveEndpointFaulted faulted)
```

#### Parameters

`faulted` [ReceiveEndpointFaulted](../../masstransit-abstractions/masstransit/receiveendpointfaulted)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
