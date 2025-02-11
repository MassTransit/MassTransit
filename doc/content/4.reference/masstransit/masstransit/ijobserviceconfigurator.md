---

title: IJobServiceConfigurator

---

# IJobServiceConfigurator

Namespace: MassTransit

```csharp
public interface IJobServiceConfigurator
```

## Properties

### **Repository**

Sets the job saga repository (default is in-memory, which is not recommended for production).
 The job repository is used to keep track of all job types, and tracking running jobs.

```csharp
public abstract ISagaRepository<JobTypeSaga> Repository { set; }
```

#### Property Value

[ISagaRepository\<JobTypeSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

### **JobRepository**

Sets the job state saga repository (default is in-memory, which is not recommended for production).
 Used to keep track of every job that was run.

```csharp
public abstract ISagaRepository<JobSaga> JobRepository { set; }
```

#### Property Value

[ISagaRepository\<JobSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

### **JobAttemptRepository**

Sets the job attempt state saga repository (default is in-memory, which is not recommended for production).
 Used to keep track of each job attempt, which may be retried based upon a retry policy.

```csharp
public abstract ISagaRepository<JobAttemptSaga> JobAttemptRepository { set; }
```

#### Property Value

[ISagaRepository\<JobAttemptSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

### **JobServiceStateEndpointName**

Override the default turnout state endpoint name (defaults to TurnoutState, turnout_state, or turnout-state)

```csharp
public abstract string JobServiceStateEndpointName { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **JobServiceJobStateEndpointName**

Override the default turnout state endpoint name (defaults to TurnoutJobState, turnout_job_state, or turnout-job-state)

```csharp
public abstract string JobServiceJobStateEndpointName { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **JobServiceJobAttemptStateEndpointName**

Override the default turnout state endpoint name (defaults to TurnoutJobAttemptState, turnout_job_attempt_state, or turnout-job-attempt-state)

```csharp
public abstract string JobServiceJobAttemptStateEndpointName { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SlotWaitTime**

The time to wait before attempting to allocate a job slot when no slots are available

```csharp
public abstract TimeSpan SlotWaitTime { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **StatusCheckInterval**

Time to wait before checking the status of a job to ensure it is still running (not dead)

```csharp
public abstract TimeSpan StatusCheckInterval { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **SuspectJobRetryCount**

The number of times to retry a suspect job before it is faulted. Defaults to zero.

```csharp
public abstract int SuspectJobRetryCount { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **SuspectJobRetryDelay**

The delay before retrying a suspect job

```csharp
public abstract TimeSpan SuspectJobRetryDelay { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **SagaPartitionCount**

If specified, overrides the default saga partition count to reduce conflicts when using optimistic concurrency.
 If using a saga repository with pessimistic concurrency, this is not recommended.

```csharp
public abstract Nullable<int> SagaPartitionCount { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **FinalizeCompleted**

If true, completed jobs are finalized, removing them from the saga repository

```csharp
public abstract bool FinalizeCompleted { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
