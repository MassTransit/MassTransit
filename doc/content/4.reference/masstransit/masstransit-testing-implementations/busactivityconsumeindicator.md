---

title: BusActivityConsumeIndicator

---

# BusActivityConsumeIndicator

Namespace: MassTransit.Testing.Implementations

```csharp
public class BusActivityConsumeIndicator : BaseBusActivityIndicatorConnectable, IObservableCondition, ICondition, ISignalResource, IConsumeObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IConditionObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [BaseBusActivityIndicatorConnectable](../masstransit-testing-implementations/basebusactivityindicatorconnectable) → [BusActivityConsumeIndicator](../masstransit-testing-implementations/busactivityconsumeindicator)<br/>
Implements [IObservableCondition](../masstransit-testing-implementations/iobservablecondition), [ICondition](../masstransit-testing-implementations/icondition), [ISignalResource](../masstransit-testing-implementations/isignalresource), [IConsumeObserver](../../masstransit-abstractions/masstransit/iconsumeobserver)

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

### **BusActivityConsumeIndicator(ISignalResource)**

```csharp
public BusActivityConsumeIndicator(ISignalResource signalResource)
```

#### Parameters

`signalResource` [ISignalResource](../masstransit-testing-implementations/isignalresource)<br/>

### **BusActivityConsumeIndicator()**

```csharp
public BusActivityConsumeIndicator()
```

## Methods

### **Signal()**

```csharp
public void Signal()
```
