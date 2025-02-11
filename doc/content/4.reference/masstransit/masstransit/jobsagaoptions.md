---

title: JobSagaOptions

---

# JobSagaOptions

Namespace: MassTransit

```csharp
public class JobSagaOptions : JobSagaSettingsConfigurator, JobSagaSettings, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobSagaOptions](../masstransit/jobsagaoptions)<br/>
Implements [JobSagaSettingsConfigurator](../masstransit-configuration/jobsagasettingsconfigurator), [JobSagaSettings](../masstransit-configuration/jobsagasettings), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **ConcurrentMessageLimit**

The number of concurrent messages

```csharp
public Nullable<int> ConcurrentMessageLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SlotWaitTime**

The time to wait for a job slot when one is unavailable

```csharp
public TimeSpan SlotWaitTime { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **StatusCheckInterval**

The time after which the status of a job should be checked

```csharp
public TimeSpan StatusCheckInterval { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **HeartbeatTimeout**

The time after which an instance will automatically be purged from the instance list

```csharp
public TimeSpan HeartbeatTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **SuspectJobRetryCount**

The number of times to retry a suspect job before it is faulted. Defaults to zero.

```csharp
public int SuspectJobRetryCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **SuspectJobRetryDelay**

The delay before retrying a suspect job

```csharp
public Nullable<TimeSpan> SuspectJobRetryDelay { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **FinalizeCompleted**

If true, completed jobs will be finalized, removing the saga from the repository

```csharp
public bool FinalizeCompleted { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **JobSagaOptions()**

```csharp
public JobSagaOptions()
```
