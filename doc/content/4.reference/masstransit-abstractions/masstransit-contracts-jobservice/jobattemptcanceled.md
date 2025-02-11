---

title: JobAttemptCanceled

---

# JobAttemptCanceled

Namespace: MassTransit.Contracts.JobService

```csharp
public interface JobAttemptCanceled
```

## Properties

### **JobId**

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **AttemptId**

```csharp
public abstract Guid AttemptId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Reason**

```csharp
public abstract string Reason { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
