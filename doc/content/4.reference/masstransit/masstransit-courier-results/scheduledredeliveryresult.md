---

title: ScheduledRedeliveryResult

---

# ScheduledRedeliveryResult

Namespace: MassTransit.Courier.Results

```csharp
public class ScheduledRedeliveryResult : ExecutionResult, CompensationResult
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduledRedeliveryResult](../masstransit-courier-results/scheduledredeliveryresult)<br/>
Implements [ExecutionResult](../../masstransit-abstractions/masstransit/executionresult), [CompensationResult](../../masstransit-abstractions/masstransit/compensationresult)

## Constructors

### **ScheduledRedeliveryResult()**

```csharp
public ScheduledRedeliveryResult()
```

## Methods

### **IsFailed(Exception)**

```csharp
public bool IsFailed(out Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Evaluate()**

```csharp
public Task Evaluate()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **IsFaulted(Exception)**

```csharp
public bool IsFaulted(out Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
