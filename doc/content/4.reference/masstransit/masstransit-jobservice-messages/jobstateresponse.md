---

title: JobStateResponse

---

# JobStateResponse

Namespace: MassTransit.JobService.Messages

```csharp
public class JobStateResponse : JobState
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobStateResponse](../masstransit-jobservice-messages/jobstateresponse)<br/>
Implements [JobState](../../masstransit-abstractions/masstransit-contracts-jobservice/jobstate)

## Properties

### **JobId**

```csharp
public Guid JobId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Submitted**

```csharp
public Nullable<DateTime> Submitted { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

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

### **LastRetryAttempt**

```csharp
public int LastRetryAttempt { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **CurrentState**

```csharp
public string CurrentState { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ProgressValue**

```csharp
public Nullable<long> ProgressValue { get; set; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ProgressLimit**

```csharp
public Nullable<long> ProgressLimit { get; set; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **JobState**

```csharp
public Dictionary<string, object> JobState { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **NextStartDate**

```csharp
public Nullable<DateTime> NextStartDate { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **IsRecurring**

```csharp
public bool IsRecurring { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **StartDate**

```csharp
public Nullable<DateTime> StartDate { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **EndDate**

```csharp
public Nullable<DateTime> EndDate { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **JobStateResponse()**

```csharp
public JobStateResponse()
```
