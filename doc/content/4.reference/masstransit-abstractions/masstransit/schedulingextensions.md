---

title: SchedulingExtensions

---

# SchedulingExtensions

Namespace: MassTransit

```csharp
public static class SchedulingExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SchedulingExtensions](../masstransit/schedulingextensions)

## Methods

### **GetSchedulingTokenId(ConsumeContext)**

```csharp
public static Nullable<Guid> GetSchedulingTokenId(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetQuartzScheduled(ConsumeContext)**

```csharp
public static Nullable<DateTimeOffset> GetQuartzScheduled(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetQuartzSent(ConsumeContext)**

```csharp
public static Nullable<DateTimeOffset> GetQuartzSent(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetQuartzNextScheduled(ConsumeContext)**

```csharp
public static Nullable<DateTimeOffset> GetQuartzNextScheduled(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetQuartzPreviousSent(ConsumeContext)**

```csharp
public static Nullable<DateTimeOffset> GetQuartzPreviousSent(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetQuartzScheduleId(ConsumeContext)**

```csharp
public static string GetQuartzScheduleId(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetQuartzScheduleGroup(ConsumeContext)**

```csharp
public static string GetQuartzScheduleGroup(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
