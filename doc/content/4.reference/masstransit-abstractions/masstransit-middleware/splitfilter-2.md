---

title: SplitFilter<TInput, TSplit>

---

# SplitFilter\<TInput, TSplit\>

Namespace: MassTransit.Middleware

```csharp
public class SplitFilter<TInput, TSplit> : IFilter<TInput>, IProbeSite
```

#### Type Parameters

`TInput`<br/>

`TSplit`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SplitFilter\<TInput, TSplit\>](../masstransit-middleware/splitfilter-2)<br/>
Implements [IFilter\<TInput\>](../masstransit/ifilter-1), [IProbeSite](../masstransit/iprobesite)

## Constructors

### **SplitFilter(IFilter\<TSplit\>, MergeFilterContextProvider\<TInput, TSplit\>, FilterContextProvider\<TSplit, TInput\>)**

```csharp
public SplitFilter(IFilter<TSplit> split, MergeFilterContextProvider<TInput, TSplit> contextProvider, FilterContextProvider<TSplit, TInput> inputContextProvider)
```

#### Parameters

`split` [IFilter\<TSplit\>](../masstransit/ifilter-1)<br/>

`contextProvider` [MergeFilterContextProvider\<TInput, TSplit\>](../masstransit/mergefiltercontextprovider-2)<br/>

`inputContextProvider` [FilterContextProvider\<TSplit, TInput\>](../masstransit/filtercontextprovider-2)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../masstransit/probecontext)<br/>

### **Send(TInput, IPipe\<TInput\>)**

```csharp
public Task Send(TInput context, IPipe<TInput> next)
```

#### Parameters

`context` TInput<br/>

`next` [IPipe\<TInput\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
