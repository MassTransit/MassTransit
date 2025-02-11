---

title: JobSlotReleased

---

# JobSlotReleased

Namespace: MassTransit.Contracts.JobService

```csharp
public interface JobSlotReleased
```

## Properties

### **JobTypeId**

```csharp
public abstract Guid JobTypeId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **JobId**

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Disposition**

```csharp
public abstract JobSlotDisposition Disposition { get; }
```

#### Property Value

[JobSlotDisposition](../masstransit-contracts-jobservice/jobslotdisposition)<br/>
