---

title: FutureResult<TCommand, TResult>

---

# FutureResult\<TCommand, TResult\>

Namespace: MassTransit.Futures

```csharp
public class FutureResult<TCommand, TResult> : ISpecification
```

#### Type Parameters

`TCommand`<br/>

`TResult`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureResult\<TCommand, TResult\>](../masstransit-futures/futureresult-2)<br/>
Implements [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **Factory**

```csharp
public ContextMessageFactory<BehaviorContext<FutureState>, TResult> Factory { set; }
```

#### Property Value

[ContextMessageFactory\<BehaviorContext\<FutureState\>, TResult\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

## Constructors

### **FutureResult()**

```csharp
public FutureResult()
```

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **SetResult(BehaviorContext\<FutureState\>)**

```csharp
public Task SetResult(BehaviorContext<FutureState> context)
```

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
