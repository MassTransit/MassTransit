---

title: FutureFault<TFault>

---

# FutureFault\<TFault\>

Namespace: MassTransit.Futures

```csharp
public class FutureFault<TFault> : ISpecification
```

#### Type Parameters

`TFault`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureFault\<TFault\>](../masstransit-futures/futurefault-1)<br/>
Implements [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **Factory**

```csharp
public ContextMessageFactory<BehaviorContext<FutureState>, TFault> Factory { set; }
```

#### Property Value

[ContextMessageFactory\<BehaviorContext\<FutureState\>, TFault\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

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

### **SetFaulted(BehaviorContext\<FutureState\>)**

```csharp
public Task SetFaulted(BehaviorContext<FutureState> context)
```

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
