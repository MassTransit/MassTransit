---

title: ActiveJob

---

# ActiveJob

Namespace: MassTransit

Active Jobs are allocated a concurrency slot, and are valid until the deadline is reached, after
 which they may be automatically released.

```csharp
public class ActiveJob : IEquatable<ActiveJob>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ActiveJob](../masstransit/activejob)<br/>
Implements [IEquatable\<ActiveJob\>](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **JobId**

```csharp
public Guid JobId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Deadline**

Calculated from the JobTimeout based on the time the job slot was requested, not currently used

```csharp
public DateTime Deadline { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **InstanceAddress**

The instance assigned to the job

```csharp
public Uri InstanceAddress { get; set; }
```

#### Property Value

Uri<br/>

### **Properties**

```csharp
public Dictionary<string, object> Properties { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

## Constructors

### **ActiveJob()**

```csharp
public ActiveJob()
```

## Methods

### **Equals(ActiveJob)**

```csharp
public bool Equals(ActiveJob other)
```

#### Parameters

`other` [ActiveJob](../masstransit/activejob)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
