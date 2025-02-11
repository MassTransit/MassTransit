---

title: BusTestPublishObserver

---

# BusTestPublishObserver

Namespace: MassTransit.Testing.Implementations

```csharp
public class BusTestPublishObserver : InactivityTestObserver, IDisposable, IInactivityObservationSource, IPublishObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IInactivityObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [InactivityTestObserver](../masstransit-testing-implementations/inactivitytestobserver) → [BusTestPublishObserver](../masstransit-testing-implementations/bustestpublishobserver)<br/>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable), [IInactivityObservationSource](../masstransit-testing-implementations/iinactivityobservationsource), [IPublishObserver](../../masstransit-abstractions/masstransit/ipublishobserver)

## Properties

### **Messages**

```csharp
public IPublishedMessageList Messages { get; }
```

#### Property Value

[IPublishedMessageList](../masstransit-testing/ipublishedmessagelist)<br/>

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

## Constructors

### **BusTestPublishObserver(TimeSpan, TimeSpan, CancellationToken)**

```csharp
public BusTestPublishObserver(TimeSpan timeout, TimeSpan inactivityTimout, CancellationToken testCompleted)
```

#### Parameters

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`inactivityTimout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`testCompleted` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
