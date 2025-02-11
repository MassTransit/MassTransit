---

title: FutureFault<TCommand, TFault, TInput>

---

# FutureFault\<TCommand, TFault, TInput\>

Namespace: MassTransit.Futures

```csharp
public class FutureFault<TCommand, TFault, TInput> : ISpecification
```

#### Type Parameters

`TCommand`<br/>

`TFault`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureFault\<TCommand, TFault, TInput\>](../masstransit-futures/futurefault-3)<br/>
Implements [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **Factory**

```csharp
public ContextMessageFactory<BehaviorContext<FutureState, TInput>, TFault> Factory { set; }
```

#### Property Value

[ContextMessageFactory\<BehaviorContext\<FutureState, TInput\>, TFault\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **WaitForPending**

```csharp
public bool WaitForPending { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **FutureFault()**

```csharp
public FutureFault()
```

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **SetFaulted(BehaviorContext\<FutureState, TInput\>)**

```csharp
public Task SetFaulted(BehaviorContext<FutureState, TInput> context)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TInput\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
