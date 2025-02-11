---

title: JobSagaSettings

---

# JobSagaSettings

Namespace: MassTransit.Configuration

Settings used by the job service sagas

```csharp
public interface JobSagaSettings
```

## Properties

### **JobAttemptSagaEndpointAddress**

```csharp
public abstract Uri JobAttemptSagaEndpointAddress { get; }
```

#### Property Value

Uri<br/>

### **JobSagaEndpointAddress**

```csharp
public abstract Uri JobSagaEndpointAddress { get; }
```

#### Property Value

Uri<br/>

### **JobTypeSagaEndpointAddress**

```csharp
public abstract Uri JobTypeSagaEndpointAddress { get; }
```

#### Property Value

Uri<br/>

### **StatusCheckInterval**

```csharp
public abstract TimeSpan StatusCheckInterval { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **SuspectJobRetryCount**

```csharp
public abstract int SuspectJobRetryCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **SuspectJobRetryDelay**

```csharp
public abstract Nullable<TimeSpan> SuspectJobRetryDelay { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SlotWaitTime**

```csharp
public abstract TimeSpan SlotWaitTime { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **HeartbeatTimeout**

```csharp
public abstract TimeSpan HeartbeatTimeout { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **FinalizeCompleted**

```csharp
public abstract bool FinalizeCompleted { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
