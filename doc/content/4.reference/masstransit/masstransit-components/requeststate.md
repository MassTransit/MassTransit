---

title: RequestState

---

# RequestState

Namespace: MassTransit.Components

```csharp
public class RequestState : SagaStateMachineInstance, ISaga, ISagaVersion
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestState](../masstransit-components/requeststate)<br/>
Implements [SagaStateMachineInstance](../../masstransit-abstractions/masstransit/sagastatemachineinstance), [ISaga](../../masstransit-abstractions/masstransit/isaga), [ISagaVersion](../../masstransit-abstractions/masstransit/isagaversion)

## Properties

### **CurrentState**

```csharp
public int CurrentState { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConversationId**

```csharp
public Nullable<Guid> ConversationId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ResponseAddress**

```csharp
public Uri ResponseAddress { get; set; }
```

#### Property Value

Uri<br/>

### **FaultAddress**

```csharp
public Uri FaultAddress { get; set; }
```

#### Property Value

Uri<br/>

### **ExpirationTime**

```csharp
public Nullable<DateTime> ExpirationTime { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SagaCorrelationId**

The correlationId of the original saga instance

```csharp
public Guid SagaCorrelationId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **SagaAddress**

The saga address where the request should be redelivered

```csharp
public Uri SagaAddress { get; set; }
```

#### Property Value

Uri<br/>

### **Version**

```csharp
public int Version { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **CorrelationId**

Same as RequestId from the original request

```csharp
public Guid CorrelationId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Constructors

### **RequestState()**

```csharp
public RequestState()
```
