---

title: ConsumeContextRetryPolicyContext

---

# ConsumeContextRetryPolicyContext

Namespace: MassTransit.RetryPolicies

```csharp
public class ConsumeContextRetryPolicyContext : RetryPolicyContext<ConsumeContext>, IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextRetryPolicyContext](../masstransit-retrypolicies/consumecontextretrypolicycontext)<br/>
Implements [RetryPolicyContext\<ConsumeContext\>](../../masstransit-abstractions/masstransit/retrypolicycontext-1), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Context**

```csharp
public ConsumeContext Context { get; }
```

#### Property Value

[ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

## Constructors

### **ConsumeContextRetryPolicyContext(RetryPolicyContext\<ConsumeContext\>, RetryConsumeContext, CancellationToken)**

```csharp
public ConsumeContextRetryPolicyContext(RetryPolicyContext<ConsumeContext> policyContext, RetryConsumeContext context, CancellationToken cancellationToken)
```

#### Parameters

`policyContext` [RetryPolicyContext\<ConsumeContext\>](../../masstransit-abstractions/masstransit/retrypolicycontext-1)<br/>

`context` [RetryConsumeContext](../masstransit-retrypolicies/retryconsumecontext)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Cancel()**

```csharp
public void Cancel()
```

### **CanRetry(Exception, RetryContext\<ConsumeContext\>)**

```csharp
public bool CanRetry(Exception exception, out RetryContext<ConsumeContext> retryContext)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`retryContext` [RetryContext\<ConsumeContext\>](../../masstransit-abstractions/masstransit/retrycontext-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **RetryFaulted(Exception)**

```csharp
public Task RetryFaulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Dispose()**

```csharp
public void Dispose()
```
