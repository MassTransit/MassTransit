---

title: JobSaga

---

# JobSaga

Namespace: MassTransit

Individual turnout jobs are tracked by this state

```csharp
public class JobSaga : SagaStateMachineInstance, ISaga, ISagaVersion
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobSaga](../masstransit/jobsaga)<br/>
Implements [SagaStateMachineInstance](../../masstransit-abstractions/masstransit/sagastatemachineinstance), [ISaga](../../masstransit-abstractions/masstransit/isaga), [ISagaVersion](../../masstransit-abstractions/masstransit/isagaversion)

## Properties

### **CurrentState**

```csharp
public int CurrentState { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Submitted**

```csharp
public Nullable<DateTime> Submitted { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ServiceAddress**

```csharp
public Uri ServiceAddress { get; set; }
```

#### Property Value

Uri<br/>

### **JobTimeout**

```csharp
public Nullable<TimeSpan> JobTimeout { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Job**

```csharp
public Dictionary<string, object> Job { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **JobTypeId**

```csharp
public Guid JobTypeId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **AttemptId**

```csharp
public Guid AttemptId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **RetryAttempt**

```csharp
public int RetryAttempt { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Started**

```csharp
public Nullable<DateTime> Started { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Completed**

```csharp
public Nullable<DateTime> Completed { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Duration**

```csharp
public Nullable<TimeSpan> Duration { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Faulted**

```csharp
public Nullable<DateTime> Faulted { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Reason**

```csharp
public string Reason { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **JobSlotWaitToken**

```csharp
public Nullable<Guid> JobSlotWaitToken { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **JobRetryDelayToken**

```csharp
public Nullable<Guid> JobRetryDelayToken { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **IncompleteAttempts**

If present, keeps track of any previously faulted attempts so that the faulted job attempt saga instances can be removed when finalized

```csharp
public List<Guid> IncompleteAttempts { get; set; }
```

#### Property Value

[List\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

### **LastProgressValue**

If present, the last reported progress value

```csharp
public Nullable<long> LastProgressValue { get; set; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LastProgressLimit**

If present, the maximum value (can be used to show a percentage)

```csharp
public Nullable<long> LastProgressLimit { get; set; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LastProgressSequenceNumber**

The last reported sequence number for the current job attempt

```csharp
public Nullable<long> LastProgressSequenceNumber { get; set; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **JobState**

The job state, saved from a previous job attempt

```csharp
public Dictionary<string, object> JobState { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **JobProperties**

The job properties, supplied by the submitted job

```csharp
public Dictionary<string, object> JobProperties { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **CronExpression**

For recurring jobs, the cron expression used to determine the next start date after the job has completed.

```csharp
public string CronExpression { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **TimeZoneId**

The time zone for the cron expression

```csharp
public string TimeZoneId { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **StartDate**

If a state date is specified, the job won't start until after the start date.

```csharp
public Nullable<DateTimeOffset> StartDate { get; set; }
```

#### Property Value

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **EndDate**

For recurring jobs, if the [JobSaga.NextStartDate](jobsaga#nextstartdate) is after the end date the job will be completed.

```csharp
public Nullable<DateTimeOffset> EndDate { get; set; }
```

#### Property Value

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **NextStartDate**

For recurring jobs, the next start date based on the cron expression (and [JobSaga.StartDate](jobsaga#startdate), if specified).

```csharp
public Nullable<DateTimeOffset> NextStartDate { get; set; }
```

#### Property Value

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RowVersion**

```csharp
public Byte[] RowVersion { get; set; }
```

#### Property Value

[Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

### **Version**

```csharp
public int Version { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **CorrelationId**

```csharp
public Guid CorrelationId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Constructors

### **JobSaga()**

```csharp
public JobSaga()
```
