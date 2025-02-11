---

title: FutureRequest<TInput, TRequest>

---

# FutureRequest\<TInput, TRequest\>

Namespace: MassTransit.Futures

```csharp
public class FutureRequest<TInput, TRequest> : ISpecification
```

#### Type Parameters

`TInput`<br/>

`TRequest`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureRequest\<TInput, TRequest\>](../masstransit-futures/futurerequest-2)<br/>
Implements [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **AddressProvider**

```csharp
public RequestAddressProvider<TInput> AddressProvider { get; set; }
```

#### Property Value

[RequestAddressProvider\<TInput\>](../masstransit/requestaddressprovider-1)<br/>

### **PendingRequestIdProvider**

```csharp
public PendingFutureIdProvider<TRequest> PendingRequestIdProvider { get; set; }
```

#### Property Value

[PendingFutureIdProvider\<TRequest\>](../masstransit/pendingfutureidprovider-1)<br/>

### **Factory**

```csharp
public ContextMessageFactory<BehaviorContext<FutureState, TInput>, TRequest> Factory { set; }
```

#### Property Value

[ContextMessageFactory\<BehaviorContext\<FutureState, TInput\>, TRequest\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

## Constructors

### **FutureRequest()**

```csharp
public FutureRequest()
```

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **SendRequest(BehaviorContext\<FutureState, TInput\>)**

```csharp
public Task SendRequest(BehaviorContext<FutureState, TInput> context)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TInput\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
