---

title: JobMetadataCache<TConsumer, TJob>

---

# JobMetadataCache\<TConsumer, TJob\>

Namespace: MassTransit.JobService

```csharp
public static class JobMetadataCache<TConsumer, TJob>
```

#### Type Parameters

`TConsumer`<br/>

`TJob`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobMetadataCache\<TConsumer, TJob\>](../masstransit-jobservice/jobmetadatacache-2)

## Methods

### **GenerateJobTypeId(String)**

```csharp
public static Guid GenerateJobTypeId(string queueName)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **GenerateJobTypeName(String)**

```csharp
public static string GenerateJobTypeName(string queueName)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
