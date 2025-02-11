---

title: JobState

---

# JobState

Namespace: MassTransit.Contracts.JobService

```csharp
public interface JobState
```

## Properties

### **JobId**

The job identifier

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Submitted**

When the job was submitted

```csharp
public abstract Nullable<DateTime> Submitted { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Started**

When the job was started, if it has started

```csharp
public abstract Nullable<DateTime> Started { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Completed**

When the job completed, if it completed

```csharp
public abstract Nullable<DateTime> Completed { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Duration**

If the job has completed, the duration of the job

```csharp
public abstract Nullable<TimeSpan> Duration { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Faulted**

When the job faulted, if it faulted

```csharp
public abstract Nullable<DateTime> Faulted { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Reason**

The fault reason, if it faulted

```csharp
public abstract string Reason { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **LastRetryAttempt**

If the job has been retried, will be &gt; 0

```csharp
public abstract int LastRetryAttempt { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **CurrentState**

The current job state

```csharp
public abstract string CurrentState { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ProgressValue**

The last reported progress value, if it's actually reported

```csharp
public abstract Nullable<long> ProgressValue { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ProgressLimit**

The last reported progress limit, if it's actually reported

```csharp
public abstract Nullable<long> ProgressLimit { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **JobState**

The state of the job, as a dictionary. Use GetJobState{T} to get the job state

```csharp
public abstract Dictionary<string, object> JobState { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **NextStartDate**

If present, the next scheduled time for the job to run

```csharp
public abstract Nullable<DateTime> NextStartDate { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **IsRecurring**

True if the job is a recurring job

```csharp
public abstract bool IsRecurring { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **StartDate**

If specified, the start date or the start of the data range (for recurring jobs) when the job should be run

```csharp
public abstract Nullable<DateTime> StartDate { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **EndDate**

If specified, the end of the data range (for recurring jobs) when the job should no longer be run

```csharp
public abstract Nullable<DateTime> EndDate { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
