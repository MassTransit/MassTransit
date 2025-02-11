---

title: BaseBusActivityIndicatorConnectable

---

# BaseBusActivityIndicatorConnectable

Namespace: MassTransit.Testing.Implementations

```csharp
public abstract class BaseBusActivityIndicatorConnectable : Connectable<IConditionObserver>, IObservableCondition, ICondition
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IConditionObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [BaseBusActivityIndicatorConnectable](../masstransit-testing-implementations/basebusactivityindicatorconnectable)<br/>
Implements [IObservableCondition](../masstransit-testing-implementations/iobservablecondition), [ICondition](../masstransit-testing-implementations/icondition)

## Properties

### **IsMet**

```csharp
public abstract bool IsMet { get; }
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

## Methods

### **ConnectConditionObserver(IConditionObserver)**

```csharp
public ConnectHandle ConnectConditionObserver(IConditionObserver observer)
```

#### Parameters

`observer` [IConditionObserver](../masstransit-testing-implementations/iconditionobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConditionUpdated()**

```csharp
protected Task ConditionUpdated()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
