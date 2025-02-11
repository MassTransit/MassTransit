---

title: RetryObservable

---

# RetryObservable

Namespace: MassTransit.Observables

```csharp
public class RetryObservable : Connectable<IRetryObserver>, IRetryObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IRetryObserver\>](../masstransit-util/connectable-1) → [RetryObservable](../masstransit-observables/retryobservable)<br/>
Implements [IRetryObserver](../masstransit/iretryobserver)

## Properties

### **Connected**

```csharp
public IRetryObserver[] Connected { get; }
```

#### Property Value

[IRetryObserver[]](../masstransit/iretryobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **RetryObservable()**

```csharp
public RetryObservable()
```

## Methods

### **PostCreate\<T\>(RetryPolicyContext\<T\>)**

```csharp
public Task PostCreate<T>(RetryPolicyContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [RetryPolicyContext\<T\>](../masstransit/retrypolicycontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostFault\<T\>(RetryContext\<T\>)**

```csharp
public Task PostFault<T>(RetryContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [RetryContext\<T\>](../masstransit/retrycontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PreRetry\<T\>(RetryContext\<T\>)**

```csharp
public Task PreRetry<T>(RetryContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [RetryContext\<T\>](../masstransit/retrycontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RetryFault\<T\>(RetryContext\<T\>)**

```csharp
public Task RetryFault<T>(RetryContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [RetryContext\<T\>](../masstransit/retrycontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RetryComplete\<T\>(RetryContext\<T\>)**

```csharp
public Task RetryComplete<T>(RetryContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [RetryContext\<T\>](../masstransit/retrycontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RetryFault(RetryContext)**

```csharp
public Task RetryFault(RetryContext context)
```

#### Parameters

`context` [RetryContext](../masstransit/retrycontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
