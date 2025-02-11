---

title: AsyncInactivityObserver

---

# AsyncInactivityObserver

Namespace: MassTransit.Testing.Implementations

```csharp
public class AsyncInactivityObserver : IInactivityObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncInactivityObserver](../masstransit-testing-implementations/asyncinactivityobserver)<br/>
Implements [IInactivityObserver](../masstransit-testing-implementations/iinactivityobserver)

## Properties

### **InactivityTask**

```csharp
public Task InactivityTask { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **InactivityToken**

```csharp
public CancellationToken InactivityToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Constructors

### **AsyncInactivityObserver(TimeSpan, CancellationToken)**

```csharp
public AsyncInactivityObserver(TimeSpan timeout, CancellationToken cancellationToken)
```

#### Parameters

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Connected(IInactivityObservationSource)**

```csharp
public void Connected(IInactivityObservationSource source)
```

#### Parameters

`source` [IInactivityObservationSource](../masstransit-testing-implementations/iinactivityobservationsource)<br/>

### **NoActivity()**

```csharp
public Task NoActivity()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ForceInactive()**

```csharp
public void ForceInactive()
```
