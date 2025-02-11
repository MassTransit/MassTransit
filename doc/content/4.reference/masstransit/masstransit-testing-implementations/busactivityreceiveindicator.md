---

title: BusActivityReceiveIndicator

---

# BusActivityReceiveIndicator

Namespace: MassTransit.Testing.Implementations

An activity indicator for receive endpoint queues. Utilizes a timer that restarts on receive activity.

```csharp
public class BusActivityReceiveIndicator : BaseBusActivityIndicatorConnectable, IObservableCondition, ICondition, ISignalResource, IReceiveObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IConditionObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [BaseBusActivityIndicatorConnectable](../masstransit-testing-implementations/basebusactivityindicatorconnectable) → [BusActivityReceiveIndicator](../masstransit-testing-implementations/busactivityreceiveindicator)<br/>
Implements [IObservableCondition](../masstransit-testing-implementations/iobservablecondition), [ICondition](../masstransit-testing-implementations/icondition), [ISignalResource](../masstransit-testing-implementations/isignalresource), [IReceiveObserver](../../masstransit-abstractions/masstransit/ireceiveobserver)

## Properties

### **IsMet**

```csharp
public bool IsMet { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Connected**

```csharp
public IConditionObserver[] Connected { get; }
```

#### Property Value

[IConditionObserver[]](../masstransit-testing-implementations/iconditionobserver)<br/>

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **BusActivityReceiveIndicator(ISignalResource, TimeSpan)**

```csharp
public BusActivityReceiveIndicator(ISignalResource signalResource, TimeSpan receiveIdleTimeout)
```

#### Parameters

`signalResource` [ISignalResource](../masstransit-testing-implementations/isignalresource)<br/>

`receiveIdleTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **BusActivityReceiveIndicator(ISignalResource)**

```csharp
public BusActivityReceiveIndicator(ISignalResource signalResource)
```

#### Parameters

`signalResource` [ISignalResource](../masstransit-testing-implementations/isignalresource)<br/>

### **BusActivityReceiveIndicator(TimeSpan)**

```csharp
public BusActivityReceiveIndicator(TimeSpan receiveIdleTimeout)
```

#### Parameters

`receiveIdleTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **BusActivityReceiveIndicator()**

```csharp
public BusActivityReceiveIndicator()
```

## Methods

### **Signal()**

```csharp
public void Signal()
```
