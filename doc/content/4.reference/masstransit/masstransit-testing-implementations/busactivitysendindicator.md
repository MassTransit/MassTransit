---

title: BusActivitySendIndicator

---

# BusActivitySendIndicator

Namespace: MassTransit.Testing.Implementations

An activity indicator for send endpoints. Utilizes a timer that restarts on send activity.

```csharp
public class BusActivitySendIndicator : BaseBusActivityIndicatorConnectable, IObservableCondition, ICondition, ISignalResource
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IConditionObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [BaseBusActivityIndicatorConnectable](../masstransit-testing-implementations/basebusactivityindicatorconnectable) → [BusActivitySendIndicator](../masstransit-testing-implementations/busactivitysendindicator)<br/>
Implements [IObservableCondition](../masstransit-testing-implementations/iobservablecondition), [ICondition](../masstransit-testing-implementations/icondition), [ISignalResource](../masstransit-testing-implementations/isignalresource)

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

### **BusActivitySendIndicator(ISignalResource, TimeSpan)**

```csharp
public BusActivitySendIndicator(ISignalResource signalResource, TimeSpan receiveIdleTimeout)
```

#### Parameters

`signalResource` [ISignalResource](../masstransit-testing-implementations/isignalresource)<br/>

`receiveIdleTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **BusActivitySendIndicator(ISignalResource)**

```csharp
public BusActivitySendIndicator(ISignalResource signalResource)
```

#### Parameters

`signalResource` [ISignalResource](../masstransit-testing-implementations/isignalresource)<br/>

### **BusActivitySendIndicator(TimeSpan)**

```csharp
public BusActivitySendIndicator(TimeSpan receiveIdleTimeout)
```

#### Parameters

`receiveIdleTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **BusActivitySendIndicator()**

```csharp
public BusActivitySendIndicator()
```

## Methods

### **Signal()**

```csharp
public void Signal()
```

### **PreSend\<T\>(SendContext\<T\>)**

```csharp
public Task PreSend<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostSend\<T\>(SendContext\<T\>)**

```csharp
public Task PostSend<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendFault\<T\>(SendContext\<T\>, Exception)**

```csharp
public Task SendFault<T>(SendContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
