---

title: FutureState

---

# FutureState

Namespace: MassTransit

```csharp
public class FutureState : SagaStateMachineInstance, ISaga, ISagaVersion
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureState](../masstransit/futurestate)<br/>
Implements [SagaStateMachineInstance](../masstransit/sagastatemachineinstance), [ISaga](../masstransit/isaga), [ISagaVersion](../masstransit/isagaversion)

## Properties

### **CurrentState**

```csharp
public int CurrentState { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Created**

```csharp
public DateTime Created { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Completed**

```csharp
public Nullable<DateTime> Completed { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Faulted**

```csharp
public Nullable<DateTime> Faulted { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Location**

```csharp
public Uri Location { get; set; }
```

#### Property Value

Uri<br/>

### **Command**

```csharp
public FutureMessage Command { get; set; }
```

#### Property Value

[FutureMessage](../masstransit/futuremessage)<br/>

### **Pending**

```csharp
public HashSet<Guid> Pending { get; set; }
```

#### Property Value

[HashSet\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1)<br/>

### **Subscriptions**

```csharp
public HashSet<FutureSubscription> Subscriptions { get; set; }
```

#### Property Value

[HashSet\<FutureSubscription\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1)<br/>

### **Variables**

```csharp
public Dictionary<string, object> Variables { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **Results**

```csharp
public Dictionary<Guid, FutureMessage> Results { get; set; }
```

#### Property Value

[Dictionary\<Guid, FutureMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **Faults**

```csharp
public Dictionary<Guid, FutureMessage> Faults { get; set; }
```

#### Property Value

[Dictionary\<Guid, FutureMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

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

### **FutureState()**

```csharp
public FutureState()
```

## Methods

### **HasSubscriptions()**

```csharp
public bool HasSubscriptions()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **HasVariables()**

```csharp
public bool HasVariables()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **HasResults()**

```csharp
public bool HasResults()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **HasFaults()**

```csharp
public bool HasFaults()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **HasPending()**

```csharp
public bool HasPending()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
