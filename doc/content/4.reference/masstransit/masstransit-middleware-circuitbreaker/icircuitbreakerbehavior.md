---

title: ICircuitBreakerBehavior

---

# ICircuitBreakerBehavior

Namespace: MassTransit.Middleware.CircuitBreaker

```csharp
public interface ICircuitBreakerBehavior : IProbeSite
```

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **PreSend()**

```csharp
Task PreSend()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostSend()**

```csharp
Task PostSend()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendFault(Exception)**

```csharp
Task SendFault(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
