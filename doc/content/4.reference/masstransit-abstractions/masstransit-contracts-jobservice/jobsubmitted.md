---

title: JobSubmitted

---

# JobSubmitted

Namespace: MassTransit.Contracts.JobService

```csharp
public interface JobSubmitted
```

## Properties

### **JobId**

The job identifier

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **JobTypeId**

```csharp
public abstract Guid JobTypeId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

The time the job was submitted

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **JobTimeout**

Timeout when running job

```csharp
public abstract TimeSpan JobTimeout { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Job**

The job, as an object dictionary

```csharp
public abstract Dictionary<string, object> Job { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **JobProperties**

The job properties

```csharp
public abstract Dictionary<string, object> JobProperties { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **Schedule**

If the job is a recurring job, the schedule for the job

```csharp
public abstract RecurringJobSchedule Schedule { get; }
```

#### Property Value

[RecurringJobSchedule](../masstransit-contracts-jobservice/recurringjobschedule)<br/>
