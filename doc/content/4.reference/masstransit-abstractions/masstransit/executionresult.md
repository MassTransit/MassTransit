---

title: ExecutionResult

---

# ExecutionResult

Namespace: MassTransit

```csharp
public interface ExecutionResult
```

## Methods

### **Evaluate()**

```csharp
Task Evaluate()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **IsFaulted(Exception)**

```csharp
bool IsFaulted(out Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
