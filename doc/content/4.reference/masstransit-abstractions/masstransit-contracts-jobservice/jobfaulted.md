---

title: JobFaulted

---

# JobFaulted

Namespace: MassTransit.Contracts.JobService

Published when a job faults

```csharp
public interface JobFaulted
```

## Properties

### **JobId**

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Duration**

```csharp
public abstract Nullable<TimeSpan> Duration { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Job**

```csharp
public abstract Dictionary<string, object> Job { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **Exceptions**

```csharp
public abstract ExceptionInfo Exceptions { get; }
```

#### Property Value

[ExceptionInfo](../masstransit/exceptioninfo)<br/>
