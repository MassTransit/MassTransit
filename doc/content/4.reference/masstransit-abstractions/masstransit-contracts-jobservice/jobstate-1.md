---

title: JobState<T>

---

# JobState\<T\>

Namespace: MassTransit.Contracts.JobService

```csharp
public interface JobState<T> : JobState
```

#### Type Parameters

`T`<br/>

Implements [JobState](../masstransit-contracts-jobservice/jobstate)

## Properties

### **JobState**

The job state, if available

```csharp
public abstract T JobState { get; }
```

#### Property Value

T<br/>
