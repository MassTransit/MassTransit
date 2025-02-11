---

title: ProgressBufferSettings

---

# ProgressBufferSettings

Namespace: MassTransit

Specifies the settings for the progress buffer, which defers updating the job progress until the
 thresholds (steps or duration) have been reached.

```csharp
public class ProgressBufferSettings
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ProgressBufferSettings](../masstransit/progressbuffersettings)

## Properties

### **UpdateLimit**

The number of progress updates reported before the value is sent to the job saga

```csharp
public int UpdateLimit { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **TimeLimit**

The time period after which the progress should be reported

```csharp
public TimeSpan TimeLimit { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Constructors

### **ProgressBufferSettings()**

```csharp
public ProgressBufferSettings()
```
