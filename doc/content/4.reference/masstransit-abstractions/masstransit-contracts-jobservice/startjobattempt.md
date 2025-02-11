---

title: StartJobAttempt

---

# StartJobAttempt

Namespace: MassTransit.Contracts.JobService

```csharp
public interface StartJobAttempt
```

## Properties

### **JobId**

The job identifier

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **AttemptId**

Identifies this attempt to run the job

```csharp
public abstract Guid AttemptId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **RetryAttempt**

Zero if the job is being started for the first time, otherwise, the number of previous failures

```csharp
public abstract int RetryAttempt { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ServiceAddress**

The service address where the job can be started

```csharp
public abstract Uri ServiceAddress { get; }
```

#### Property Value

Uri<br/>

### **InstanceAddress**

The instance address of the assigned job slot instance

```csharp
public abstract Uri InstanceAddress { get; }
```

#### Property Value

Uri<br/>

### **Job**

The job, as an object dictionary

```csharp
public abstract Dictionary<string, object> Job { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **JobTypeId**

The JobTypeId, to ensure the proper job type is started

```csharp
public abstract Guid JobTypeId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **LastProgressValue**

The last reported progress value from a previous job execution

```csharp
public abstract Nullable<long> LastProgressValue { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LastProgressLimit**

The last reported progress limit

```csharp
public abstract Nullable<long> LastProgressLimit { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **JobState**

The job state, as a dictionary

```csharp
public abstract Dictionary<string, object> JobState { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **JobProperties**

The job properties, supplied when the job was submitted

```csharp
public abstract Dictionary<string, object> JobProperties { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>
