---

title: FutureResult<TCommand, TResult, TInput>

---

# FutureResult\<TCommand, TResult, TInput\>

Namespace: MassTransit.Futures

```csharp
public class FutureResult<TCommand, TResult, TInput> : ISpecification
```

#### Type Parameters

`TCommand`<br/>

`TResult`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureResult\<TCommand, TResult, TInput\>](../masstransit-futures/futureresult-3)<br/>
Implements [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **Factory**

```csharp
public ContextMessageFactory<BehaviorContext<FutureState, TInput>, TResult> Factory { set; }
```

#### Property Value

[ContextMessageFactory\<BehaviorContext\<FutureState, TInput\>, TResult\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

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

### **SetResult(BehaviorContext\<FutureState, TInput\>)**

```csharp
public Task SetResult(BehaviorContext<FutureState, TInput> context)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TInput\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
