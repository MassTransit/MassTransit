---

title: SubmitJob<TJob>

---

# SubmitJob\<TJob\>

Namespace: MassTransit.Contracts.JobService

```csharp
public interface SubmitJob<TJob>
```

#### Type Parameters

`TJob`<br/>

## Properties

### **JobId**

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Job**

```csharp
public abstract TJob Job { get; }
```

#### Property Value

TJob<br/>

### **Schedule**

```csharp
public abstract RecurringJobSchedule Schedule { get; }
```

#### Property Value

[RecurringJobSchedule](../masstransit-contracts-jobservice/recurringjobschedule)<br/>

### **Properties**

```csharp
public abstract Dictionary<string, object> Properties { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>
