---

title: IObservableCondition

---

# IObservableCondition

Namespace: MassTransit.Testing.Implementations

Represents a boolean condition which may be observed.

```csharp
public interface IObservableCondition : ICondition
```

Implements [ICondition](../masstransit-testing-implementations/icondition)

## Methods

### **ConnectConditionObserver(IConditionObserver)**

```csharp
ConnectHandle ConnectConditionObserver(IConditionObserver observer)
```

#### Parameters

`observer` [IConditionObserver](../masstransit-testing-implementations/iconditionobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
