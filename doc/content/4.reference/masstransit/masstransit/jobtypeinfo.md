---

title: JobTypeInfo

---

# JobTypeInfo

Namespace: MassTransit

```csharp
public interface JobTypeInfo
```

## Properties

### **Name**

The job type name, supplied by the job consumer

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConcurrentJobLimit**

Set the concurrent job limit. The limit is applied to each instance if the job consumer is scaled out.

```csharp
public abstract int ConcurrentJobLimit { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Properties**

Job properties configured by [JobOptions\<TJob\>](../masstransit/joboptions-1)

```csharp
public abstract IReadOnlyDictionary<string, object> Properties { get; }
```

#### Property Value

[IReadOnlyDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br/>

### **ActiveJobs**

Currently active jobs for this job type across all instances

```csharp
public abstract IReadOnlyList<ActiveJob> ActiveJobs { get; }
```

#### Property Value

[IReadOnlyList\<ActiveJob\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br/>

### **Instances**

Currently active instances for this job type, that aren't suspect/dead

```csharp
public abstract IReadOnlyDictionary<Uri, JobTypeInstance> Instances { get; }
```

#### Property Value

[IReadOnlyDictionary\<Uri, JobTypeInstance\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br/>
