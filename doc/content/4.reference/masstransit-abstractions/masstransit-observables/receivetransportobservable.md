---

title: ReceiveTransportObservable

---

# ReceiveTransportObservable

Namespace: MassTransit.Observables

```csharp
public class ReceiveTransportObservable : Connectable<IReceiveTransportObserver>, IReceiveTransportObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IReceiveTransportObserver\>](../masstransit-util/connectable-1) → [ReceiveTransportObservable](../masstransit-observables/receivetransportobservable)<br/>
Implements [IReceiveTransportObserver](../masstransit/ireceivetransportobserver)

## Properties

### **Connected**

```csharp
public IReceiveTransportObserver[] Connected { get; }
```

#### Property Value

[IReceiveTransportObserver[]](../masstransit/ireceivetransportobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **ReceiveTransportObservable()**

```csharp
public ReceiveTransportObservable()
```

## Methods

### **Ready(ReceiveTransportReady)**

```csharp
public Task Ready(ReceiveTransportReady ready)
```

#### Parameters

`ready` [ReceiveTransportReady](../masstransit/receivetransportready)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Completed(ReceiveTransportCompleted)**

```csharp
public Task Completed(ReceiveTransportCompleted completed)
```

#### Parameters

`completed` [ReceiveTransportCompleted](../masstransit/receivetransportcompleted)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted(ReceiveTransportFaulted)**

```csharp
public Task Faulted(ReceiveTransportFaulted faulted)
```

#### Parameters

`faulted` [ReceiveTransportFaulted](../masstransit/receivetransportfaulted)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
