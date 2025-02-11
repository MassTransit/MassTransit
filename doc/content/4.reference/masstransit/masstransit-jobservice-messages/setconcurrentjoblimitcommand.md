---

title: SetConcurrentJobLimitCommand

---

# SetConcurrentJobLimitCommand

Namespace: MassTransit.JobService.Messages

```csharp
public class SetConcurrentJobLimitCommand : SetConcurrentJobLimit
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SetConcurrentJobLimitCommand](../masstransit-jobservice-messages/setconcurrentjoblimitcommand)<br/>
Implements [SetConcurrentJobLimit](../../masstransit-abstractions/masstransit-contracts-jobservice/setconcurrentjoblimit)

## Properties

### **JobTypeId**

```csharp
public Guid JobTypeId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **InstanceAddress**

```csharp
public Uri InstanceAddress { get; set; }
```

#### Property Value

Uri<br/>

### **ConcurrentJobLimit**

```csharp
public int ConcurrentJobLimit { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Kind**

```csharp
public ConcurrentLimitKind Kind { get; set; }
```

#### Property Value

[ConcurrentLimitKind](../../masstransit-abstractions/masstransit-contracts-jobservice/concurrentlimitkind)<br/>

### **Duration**

```csharp
public Nullable<TimeSpan> Duration { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **JobTypeName**

```csharp
public string JobTypeName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **JobTypeProperties**

```csharp
public Dictionary<string, object> JobTypeProperties { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **InstanceProperties**

```csharp
public Dictionary<string, object> InstanceProperties { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **GlobalConcurrentJobLimit**

```csharp
public Nullable<int> GlobalConcurrentJobLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **SetConcurrentJobLimitCommand()**

```csharp
public SetConcurrentJobLimitCommand()
```
