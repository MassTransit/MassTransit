---

title: JobStateResponse<T>

---

# JobStateResponse\<T\>

Namespace: MassTransit.JobService.Messages

```csharp
public class JobStateResponse<T> : JobState<T>, JobState
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobStateResponse\<T\>](../masstransit-jobservice-messages/jobstateresponse-1)<br/>
Implements [JobState\<T\>](../../masstransit-abstractions/masstransit-contracts-jobservice/jobstate-1), [JobState](../../masstransit-abstractions/masstransit-contracts-jobservice/jobstate)

## Properties

### **JobId**

```csharp
public Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Submitted**

```csharp
public Nullable<DateTime> Submitted { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Started**

```csharp
public Nullable<DateTime> Started { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Completed**

```csharp
public Nullable<DateTime> Completed { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Duration**

```csharp
public Nullable<TimeSpan> Duration { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Faulted**

```csharp
public Nullable<DateTime> Faulted { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Reason**

```csharp
public string Reason { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **LastRetryAttempt**

```csharp
public int LastRetryAttempt { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **CurrentState**

```csharp
public string CurrentState { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ProgressValue**

```csharp
public Nullable<long> ProgressValue { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ProgressLimit**

```csharp
public Nullable<long> ProgressLimit { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **JobState**

```csharp
public Dictionary<string, object> JobState { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **NextStartDate**

```csharp
public Nullable<DateTime> NextStartDate { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **IsRecurring**

```csharp
public bool IsRecurring { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **StartDate**

```csharp
public Nullable<DateTime> StartDate { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **EndDate**

```csharp
public Nullable<DateTime> EndDate { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **JobStateResponse(JobState, T)**

```csharp
public JobStateResponse(JobState jobState, T jobStateOfT)
```

#### Parameters

`jobState` [JobState](../../masstransit-abstractions/masstransit-contracts-jobservice/jobstate)<br/>

`jobStateOfT` T<br/>
