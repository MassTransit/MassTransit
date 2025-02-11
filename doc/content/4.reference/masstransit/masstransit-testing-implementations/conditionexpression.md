---

title: ConditionExpression

---

# ConditionExpression

Namespace: MassTransit.Testing.Implementations

A collection of blocks of conditions that must occur to signal a resource.
 Each condition block in the list is logically OR'd with the other condition blocks.
 Each condition within a condition block is logically AND'd with the other conditions in the same block.

```csharp
public class ConditionExpression : IConditionObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConditionExpression](../masstransit-testing-implementations/conditionexpression)<br/>
Implements [IConditionObserver](../masstransit-testing-implementations/iconditionobserver)

## Constructors

### **ConditionExpression(ISignalResource)**

```csharp
public ConditionExpression(ISignalResource resource)
```

#### Parameters

`resource` [ISignalResource](../masstransit-testing-implementations/isignalresource)<br/>

## Methods

### **ConditionUpdated()**

```csharp
public Task ConditionUpdated()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **AddConditionBlock(IObservableCondition[])**

Adds a condition block where all conditions in the array must be logically ANDed together to succeed.

```csharp
public void AddConditionBlock(IObservableCondition[] conditions)
```

#### Parameters

`conditions` [IObservableCondition[]](../masstransit-testing-implementations/iobservablecondition)<br/>

### **ClearAllConditions()**

```csharp
public void ClearAllConditions()
```

### **CheckCondition()**

```csharp
public bool CheckCondition()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
