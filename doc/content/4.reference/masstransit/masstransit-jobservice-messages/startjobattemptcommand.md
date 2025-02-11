---

title: StartJobAttemptCommand

---

# StartJobAttemptCommand

Namespace: MassTransit.JobService.Messages

```csharp
public class StartJobAttemptCommand : StartJobAttempt
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StartJobAttemptCommand](../masstransit-jobservice-messages/startjobattemptcommand)<br/>
Implements [StartJobAttempt](../../masstransit-abstractions/masstransit-contracts-jobservice/startjobattempt)

## Properties

### **JobId**

```csharp
public Guid JobId { get; set; }
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

### **ServiceAddress**

```csharp
public Uri ServiceAddress { get; set; }
```

#### Property Value

Uri<br/>

### **InstanceAddress**

```csharp
public Uri InstanceAddress { get; set; }
```

#### Property Value

Uri<br/>

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

### **LastProgressValue**

```csharp
public Nullable<long> LastProgressValue { get; set; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LastProgressLimit**

```csharp
public Nullable<long> LastProgressLimit { get; set; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **JobState**

```csharp
public Dictionary<string, object> JobState { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **JobProperties**

```csharp
public Dictionary<string, object> JobProperties { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

## Constructors

### **StartJobAttemptCommand()**

```csharp
public StartJobAttemptCommand()
```
