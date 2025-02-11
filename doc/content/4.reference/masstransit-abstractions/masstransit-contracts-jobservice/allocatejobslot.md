---

title: AllocateJobSlot

---

# AllocateJobSlot

Namespace: MassTransit.Contracts.JobService

```csharp
public interface AllocateJobSlot
```

## Properties

### **JobTypeId**

```csharp
public abstract Guid JobTypeId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **JobTimeout**

```csharp
public abstract TimeSpan JobTimeout { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **JobId**

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **JobProperties**

The job properties

```csharp
public abstract Dictionary<string, object> JobProperties { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>
