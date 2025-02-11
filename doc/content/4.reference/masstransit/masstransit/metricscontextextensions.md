---

title: MetricsContextExtensions

---

# MetricsContextExtensions

Namespace: MassTransit

```csharp
public static class MetricsContextExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MetricsContextExtensions](../masstransit/metricscontextextensions)

## Methods

### **AddMetricTags(PipeContext, String, Object)**

Add custom tag to the metrics emitted by the library

```csharp
public static void AddMetricTags(PipeContext pipeContext, string key, object value)
```

#### Parameters

`pipeContext` [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **SetMetricTags(PipeContext, TagList)**

Set and override custom metric tags emitted by the library

```csharp
public static void SetMetricTags(PipeContext pipeContext, TagList tagList)
```

#### Parameters

`pipeContext` [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)<br/>

`tagList` TagList<br/>
