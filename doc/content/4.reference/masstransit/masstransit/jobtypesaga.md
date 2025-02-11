---

title: JobTypeSaga

---

# JobTypeSaga

Namespace: MassTransit

Every job type has one entry in this state machine

```csharp
public class JobTypeSaga : SagaStateMachineInstance, ISaga, JobTypeInfo, ISagaVersion
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobTypeSaga](../masstransit/jobtypesaga)<br/>
Implements [SagaStateMachineInstance](../../masstransit-abstractions/masstransit/sagastatemachineinstance), [ISaga](../../masstransit-abstractions/masstransit/isaga), [JobTypeInfo](../masstransit/jobtypeinfo), [ISagaVersion](../../masstransit-abstractions/masstransit/isagaversion)

## Properties

### **CurrentState**

```csharp
public int CurrentState { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ActiveJobCount**

```csharp
public int ActiveJobCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrentJobLimit**

The concurrent job limit, which is configured by the job options. Initially, it defaults to one when the state machine
 is created. Once a service endpoint starts, that endpoint sends a command to set the configure concurrent job limit.

```csharp
public int ConcurrentJobLimit { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **OverrideJobLimit**

The job limit may be overridden temporarily, to either reduce or increase the number of concurrent jobs. Once the
 override job limit expires, the concurrent job limit returns to the original value.

```csharp
public Nullable<int> OverrideJobLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **OverrideLimitExpiration**

If an [JobTypeSaga.OverrideJobLimit](jobtypesaga#overridejoblimit) is specified, the time when the override job limit expires

```csharp
public Nullable<DateTime> OverrideLimitExpiration { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ActiveJobs**

The last known active jobs

```csharp
public List<ActiveJob> ActiveJobs { get; set; }
```

#### Property Value

[List\<ActiveJob\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

### **Instances**

Tracks the instances, when they were last updated

```csharp
public Dictionary<Uri, JobTypeInstance> Instances { get; set; }
```

#### Property Value

[Dictionary\<Uri, JobTypeInstance\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **Properties**

Job properties passed by the [JobOptions\<TJob\>](../masstransit/joboptions-1) configuration

```csharp
public Dictionary<string, object> Properties { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **RowVersion**

```csharp
public Byte[] RowVersion { get; set; }
```

#### Property Value

[Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

### **GlobalConcurrentJobLimit**

```csharp
public Nullable<int> GlobalConcurrentJobLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Version**

```csharp
public int Version { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Name**

The name of the job type

```csharp
public string Name { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **CorrelationId**

The MD5 hash of the job type

```csharp
public Guid CorrelationId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Constructors

### **JobTypeSaga()**

```csharp
public JobTypeSaga()
```
