---

title: InMemoryOutboxFilter<TContext, TResult>

---

# InMemoryOutboxFilter\<TContext, TResult\>

Namespace: MassTransit.Middleware

```csharp
public class InMemoryOutboxFilter<TContext, TResult> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

`TResult`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryOutboxFilter\<TContext, TResult\>](../masstransit-middleware/inmemoryoutboxfilter-2)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **InMemoryOutboxFilter(ISetScopedConsumeContext, Func\<TContext, TResult\>, Boolean)**

```csharp
public InMemoryOutboxFilter(ISetScopedConsumeContext setter, Func<TContext, TResult> contextFactory, bool concurrentMessageDelivery)
```

#### Parameters

`setter` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

`contextFactory` [Func\<TContext, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`concurrentMessageDelivery` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **Send(TContext, IPipe\<TContext\>)**

```csharp
public Task Send(TContext context, IPipe<TContext> next)
```

#### Parameters

`context` TContext<br/>

`next` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Method1()**

```csharp
public void Method1()
```

### **Method2()**

```csharp
public void Method2()
```

### **Method3()**

```csharp
public void Method3()
```
