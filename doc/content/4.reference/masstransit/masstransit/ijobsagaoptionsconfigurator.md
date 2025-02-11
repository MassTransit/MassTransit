---

title: IJobSagaOptionsConfigurator

---

# IJobSagaOptionsConfigurator

Namespace: MassTransit

```csharp
public interface IJobSagaOptionsConfigurator
```

## Properties

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

### **SlotRequestTimeout**

Timeout on request to allocate a job slot

```csharp
public abstract TimeSpan SlotRequestTimeout { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **StartJobTimeout**

Timeout to wait for a job to start

```csharp
public abstract TimeSpan StartJobTimeout { set; }
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
