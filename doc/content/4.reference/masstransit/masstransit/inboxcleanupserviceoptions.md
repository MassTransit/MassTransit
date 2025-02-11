---

title: InboxCleanupServiceOptions

---

# InboxCleanupServiceOptions

Namespace: MassTransit

```csharp
public class InboxCleanupServiceOptions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InboxCleanupServiceOptions](../masstransit/inboxcleanupserviceoptions)

## Properties

### **DuplicateDetectionWindow**

The amount of time a message remaining in the Inbox

```csharp
public TimeSpan DuplicateDetectionWindow { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **QueryMessageLimit**

The maximum number of messages to load and remove at a time that meet the criteria

```csharp
public int QueryMessageLimit { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **QueryTimeout**

Database query timeout for loading/removing messages

```csharp
public TimeSpan QueryTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **QueryDelay**

Delay between each database sweep to cleanup the inbox

```csharp
public TimeSpan QueryDelay { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Constructors

### **InboxCleanupServiceOptions()**

```csharp
public InboxCleanupServiceOptions()
```
