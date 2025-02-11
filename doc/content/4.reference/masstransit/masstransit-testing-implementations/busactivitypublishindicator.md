---

title: BusActivityPublishIndicator

---

# BusActivityPublishIndicator

Namespace: MassTransit.Testing.Implementations

An activity indicator for publish endpoints. Utilizes a timer that restarts on publish activity.

```csharp
public class BusActivityPublishIndicator : BaseBusActivityIndicatorConnectable, IObservableCondition, ICondition, ISignalResource
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IConditionObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [BaseBusActivityIndicatorConnectable](../masstransit-testing-implementations/basebusactivityindicatorconnectable) → [BusActivityPublishIndicator](../masstransit-testing-implementations/busactivitypublishindicator)<br/>
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

### **BusActivityPublishIndicator(ISignalResource, TimeSpan)**

```csharp
public BusActivityPublishIndicator(ISignalResource signalResource, TimeSpan receiveIdleTimeout)
```

#### Parameters

`signalResource` [ISignalResource](../masstransit-testing-implementations/isignalresource)<br/>

`receiveIdleTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **BusActivityPublishIndicator(ISignalResource)**

```csharp
public BusActivityPublishIndicator(ISignalResource signalResource)
```

#### Parameters

`signalResource` [ISignalResource](../masstransit-testing-implementations/isignalresource)<br/>

### **BusActivityPublishIndicator(TimeSpan)**

```csharp
public BusActivityPublishIndicator(TimeSpan receiveIdleTimeout)
```

#### Parameters

`receiveIdleTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **BusActivityPublishIndicator()**

```csharp
public BusActivityPublishIndicator()
```

## Methods

### **Signal()**

```csharp
public void Signal()
```

### **PrePublish\<T\>(PublishContext\<T\>)**

```csharp
public Task PrePublish<T>(PublishContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [PublishContext\<T\>](../../masstransit-abstractions/masstransit/publishcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostPublish\<T\>(PublishContext\<T\>)**

```csharp
public Task PostPublish<T>(PublishContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [PublishContext\<T\>](../../masstransit-abstractions/masstransit/publishcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishFault\<T\>(PublishContext\<T\>, Exception)**

```csharp
public Task PublishFault<T>(PublishContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [PublishContext\<T\>](../../masstransit-abstractions/masstransit/publishcontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
