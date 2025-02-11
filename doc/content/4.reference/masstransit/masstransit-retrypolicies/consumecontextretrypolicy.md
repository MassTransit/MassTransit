---

title: ConsumeContextRetryPolicy

---

# ConsumeContextRetryPolicy

Namespace: MassTransit.RetryPolicies

```csharp
public class ConsumeContextRetryPolicy : IRetryPolicy, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextRetryPolicy](../masstransit-retrypolicies/consumecontextretrypolicy)<br/>
Implements [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ConsumeContextRetryPolicy(IRetryPolicy, CancellationToken)**

```csharp
public ConsumeContextRetryPolicy(IRetryPolicy retryPolicy, CancellationToken cancellationToken)
```

#### Parameters

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **IsHandled(Exception)**

```csharp
public bool IsHandled(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
