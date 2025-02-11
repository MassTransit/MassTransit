---

title: SetConcurrentJobLimit

---

# SetConcurrentJobLimit

Namespace: MassTransit.Contracts.JobService

When the bus is started, the current job limit for a job type is published along with the instance address.

```csharp
public interface SetConcurrentJobLimit
```

## Properties

### **JobTypeId**

```csharp
public abstract Guid JobTypeId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **InstanceAddress**

```csharp
public abstract Uri InstanceAddress { get; }
```

#### Property Value

Uri<br/>

### **ConcurrentJobLimit**

```csharp
public abstract int ConcurrentJobLimit { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Kind**

```csharp
public abstract ConcurrentLimitKind Kind { get; }
```

#### Property Value

[ConcurrentLimitKind](../masstransit-contracts-jobservice/concurrentlimitkind)<br/>

### **Duration**

How long a overridden limit should be in effect

```csharp
public abstract Nullable<TimeSpan> Duration { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **JobTypeName**

If present, the job type name

```csharp
public abstract string JobTypeName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **JobTypeProperties**

Allows properties to be submitted by the job service instance that can be used by the job distribution strategy

```csharp
public abstract Dictionary<string, object> JobTypeProperties { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **InstanceProperties**

Allows properties to be submitted by the job service instance that can be used by the job distribution strategy

```csharp
public abstract Dictionary<string, object> InstanceProperties { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **GlobalConcurrentJobLimit**

If configured, specifies a global limit across all job consumer instances

```csharp
public abstract Nullable<int> GlobalConcurrentJobLimit { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
