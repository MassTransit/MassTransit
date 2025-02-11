---

title: InactivityTestObserver

---

# InactivityTestObserver

Namespace: MassTransit.Testing.Implementations

```csharp
public abstract class InactivityTestObserver : Connectable<IInactivityObserver>, IDisposable, IInactivityObservationSource
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IInactivityObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [InactivityTestObserver](../masstransit-testing-implementations/inactivitytestobserver)<br/>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable), [IInactivityObservationSource](../masstransit-testing-implementations/iinactivityobservationsource)

## Properties

### **IsInactive**

```csharp
public bool IsInactive { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Connected**

```csharp
public IInactivityObserver[] Connected { get; }
```

#### Property Value

[IInactivityObserver[]](../masstransit-testing-implementations/iinactivityobserver)<br/>

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **Dispose()**

```csharp
public void Dispose()
```

### **ConnectInactivityObserver(IInactivityObserver)**

```csharp
public ConnectHandle ConnectInactivityObserver(IInactivityObserver observer)
```

#### Parameters

`observer` [IInactivityObserver](../masstransit-testing-implementations/iinactivityobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **StartTimer(TimeSpan)**

```csharp
protected void StartTimer(TimeSpan inactivityTimout)
```

#### Parameters

`inactivityTimout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **RestartTimer(Boolean)**

```csharp
public Task RestartTimer(bool activityDetected)
```

#### Parameters

`activityDetected` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyInactive()**

```csharp
protected Task NotifyInactive()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
