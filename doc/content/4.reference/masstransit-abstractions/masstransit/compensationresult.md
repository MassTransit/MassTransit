---

title: CompensationResult

---

# CompensationResult

Namespace: MassTransit

```csharp
public interface CompensationResult
```

## Methods

### **Evaluate()**

```csharp
Task Evaluate()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **IsFailed(Exception)**

```csharp
bool IsFailed(out Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
