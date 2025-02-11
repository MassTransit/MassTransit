---

title: JobAttemptSaga

---

# JobAttemptSaga

Namespace: MassTransit

Each attempt to run a job is tracked by this state

```csharp
public class JobAttemptSaga : SagaStateMachineInstance, ISaga, ISagaVersion
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobAttemptSaga](../masstransit/jobattemptsaga)<br/>
Implements [SagaStateMachineInstance](../../masstransit-abstractions/masstransit/sagastatemachineinstance), [ISaga](../../masstransit-abstractions/masstransit/isaga), [ISagaVersion](../../masstransit-abstractions/masstransit/isagaversion)

## Properties

### **CurrentState**

```csharp
public int CurrentState { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **JobId**

```csharp
public Guid JobId { get; set; }
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

### **Started**

```csharp
public Nullable<DateTime> Started { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Faulted**

```csharp
public Nullable<DateTime> Faulted { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StatusCheckTokenId**

```csharp
public Nullable<Guid> StatusCheckTokenId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RowVersion**

```csharp
public Byte[] RowVersion { get; set; }
```

#### Property Value

[Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

### **Version**

```csharp
public int Version { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **CorrelationId**

```csharp
public Guid CorrelationId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Constructors

### **JobAttemptSaga()**

```csharp
public JobAttemptSaga()
```
