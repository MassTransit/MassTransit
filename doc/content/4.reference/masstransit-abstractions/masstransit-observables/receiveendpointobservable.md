---

title: ReceiveEndpointObservable

---

# ReceiveEndpointObservable

Namespace: MassTransit.Observables

```csharp
public class ReceiveEndpointObservable : Connectable<IReceiveEndpointObserver>, IReceiveEndpointObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IReceiveEndpointObserver\>](../masstransit-util/connectable-1) → [ReceiveEndpointObservable](../masstransit-observables/receiveendpointobservable)<br/>
Implements [IReceiveEndpointObserver](../masstransit/ireceiveendpointobserver)

## Properties

### **Connected**

```csharp
public IReceiveEndpointObserver[] Connected { get; }
```

#### Property Value

[IReceiveEndpointObserver[]](../masstransit/ireceiveendpointobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **ReceiveEndpointObservable()**

```csharp
public ReceiveEndpointObservable()
```

## Methods

### **Ready(ReceiveEndpointReady)**

```csharp
public Task Ready(ReceiveEndpointReady ready)
```

#### Parameters

`ready` [ReceiveEndpointReady](../masstransit/receiveendpointready)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stopping(ReceiveEndpointStopping)**

```csharp
public Task Stopping(ReceiveEndpointStopping stopping)
```

#### Parameters

`stopping` [ReceiveEndpointStopping](../masstransit/receiveendpointstopping)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Completed(ReceiveEndpointCompleted)**

```csharp
public Task Completed(ReceiveEndpointCompleted completed)
```

#### Parameters

`completed` [ReceiveEndpointCompleted](../masstransit/receiveendpointcompleted)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted(ReceiveEndpointFaulted)**

```csharp
public Task Faulted(ReceiveEndpointFaulted faulted)
```

#### Parameters

`faulted` [ReceiveEndpointFaulted](../masstransit/receiveendpointfaulted)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
