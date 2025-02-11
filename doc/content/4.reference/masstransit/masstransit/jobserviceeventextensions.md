---

title: JobServiceEventExtensions

---

# JobServiceEventExtensions

Namespace: MassTransit

```csharp
public static class JobServiceEventExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobServiceEventExtensions](../masstransit/jobserviceeventextensions)

## Methods

### **GetJob\<TJob\>(ConsumeContext\<StartJob\>)**

Returns the job from the message

```csharp
public static TJob GetJob<TJob>(ConsumeContext<StartJob> context)
```

#### Type Parameters

`TJob`<br/>

#### Parameters

`context` [ConsumeContext\<StartJob\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

TJob<br/>

### **GetJob\<TJob\>(ConsumeContext\<FaultJob\>)**

```csharp
public static TJob GetJob<TJob>(ConsumeContext<FaultJob> context)
```

#### Type Parameters

`TJob`<br/>

#### Parameters

`context` [ConsumeContext\<FaultJob\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

TJob<br/>

### **GetJob\<TJob\>(ConsumeContext\<CompleteJob\>)**

```csharp
public static TJob GetJob<TJob>(ConsumeContext<CompleteJob> context)
```

#### Type Parameters

`TJob`<br/>

#### Parameters

`context` [ConsumeContext\<CompleteJob\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

TJob<br/>

### **GetJob\<TJob\>(ConsumeContext\<JobCompleted\>)**

```csharp
public static TJob GetJob<TJob>(ConsumeContext<JobCompleted> context)
```

#### Type Parameters

`TJob`<br/>

#### Parameters

`context` [ConsumeContext\<JobCompleted\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

TJob<br/>

### **GetResult\<TResult\>(ConsumeContext\<JobCompleted\>)**

```csharp
public static TResult GetResult<TResult>(ConsumeContext<JobCompleted> context)
```

#### Type Parameters

`TResult`<br/>

#### Parameters

`context` [ConsumeContext\<JobCompleted\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

TResult<br/>

### **GetJob\<TJob\>(ConsumeContext\<JobFaulted\>)**

```csharp
public static TJob GetJob<TJob>(ConsumeContext<JobFaulted> context)
```

#### Type Parameters

`TJob`<br/>

#### Parameters

`context` [ConsumeContext\<JobFaulted\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

TJob<br/>
