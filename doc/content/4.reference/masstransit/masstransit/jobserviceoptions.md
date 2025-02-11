---

title: JobServiceOptions

---

# JobServiceOptions

Namespace: MassTransit

```csharp
public class JobServiceOptions : JobSagaSettings, IOptions, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobServiceOptions](../masstransit/jobserviceoptions)<br/>
Implements [JobSagaSettings](../masstransit-configuration/jobsagasettings), [IOptions](../../masstransit-abstractions/masstransit-configuration/ioptions), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **JobTypeSagaEndpointName**

```csharp
public string JobTypeSagaEndpointName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **JobStateSagaEndpointName**

```csharp
public string JobStateSagaEndpointName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **JobAttemptSagaEndpointName**

```csharp
public string JobAttemptSagaEndpointName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **JobService**

The job service for the endpoint

```csharp
public IJobService JobService { get; set; }
```

#### Property Value

[IJobService](../masstransit-jobservice/ijobservice)<br/>

### **HeartbeatInterval**

How often a job instance should send a heartbeat

```csharp
public TimeSpan HeartbeatInterval { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **SagaPartitionCount**

If specified, overrides the default saga partition count to reduce conflicts when using optimistic concurrency.
 If using a saga repository with pessimistic concurrency, this is not recommended.

```csharp
public Nullable<int> SagaPartitionCount { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InstanceEndpointConfigurator**

```csharp
public IReceiveEndpointConfigurator InstanceEndpointConfigurator { get; set; }
```

#### Property Value

[IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **OnConfigureEndpoint**

```csharp
public Action<IReceiveEndpointConfigurator> OnConfigureEndpoint { get; set; }
```

#### Property Value

[Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **JobSagaEndpointAddress**

The endpoint for the JobAttemptStateMachine

```csharp
public Uri JobSagaEndpointAddress { get; set; }
```

#### Property Value

Uri<br/>

### **JobTypeSagaEndpointAddress**

The endpoint for the JobAttemptStateMachine

```csharp
public Uri JobTypeSagaEndpointAddress { get; set; }
```

#### Property Value

Uri<br/>

### **JobAttemptSagaEndpointAddress**

The endpoint for the JobAttemptStateMachine

```csharp
public Uri JobAttemptSagaEndpointAddress { get; set; }
```

#### Property Value

Uri<br/>

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

### **JobServiceOptions()**

```csharp
public JobServiceOptions()
```
