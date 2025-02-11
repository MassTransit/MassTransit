---

title: JobMetadataCache<TJob>

---

# JobMetadataCache\<TJob\>

Namespace: MassTransit.JobService

```csharp
public static class JobMetadataCache<TJob>
```

#### Type Parameters

`TJob`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobMetadataCache\<TJob\>](../masstransit-jobservice/jobmetadatacache-1)

## Methods

### **GenerateRecurringJobId(String)**

```csharp
public static Guid GenerateRecurringJobId(string jobName)
```

#### Parameters

`jobName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **GenerateJobTypeName(String)**

```csharp
public static string GenerateJobTypeName(string jobName)
```

#### Parameters

`jobName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
