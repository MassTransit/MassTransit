---

title: MergePipe<TInput, TSplit>

---

# MergePipe\<TInput, TSplit\>

Namespace: MassTransit.Middleware

```csharp
public class MergePipe<TInput, TSplit> : IPipe<TSplit>, IProbeSite
```

#### Type Parameters

`TInput`<br/>

`TSplit`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MergePipe\<TInput, TSplit\>](../masstransit-middleware/mergepipe-2)<br/>
Implements [IPipe\<TSplit\>](../masstransit/ipipe-1), [IProbeSite](../masstransit/iprobesite)

## Constructors

### **MergePipe(IPipe\<TInput\>, TInput, MergeFilterContextProvider\<TInput, TSplit\>)**

```csharp
public MergePipe(IPipe<TInput> next, TInput input, MergeFilterContextProvider<TInput, TSplit> contextProvider)
```

#### Parameters

`next` [IPipe\<TInput\>](../masstransit/ipipe-1)<br/>

`input` TInput<br/>

`contextProvider` [MergeFilterContextProvider\<TInput, TSplit\>](../masstransit/mergefiltercontextprovider-2)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../masstransit/probecontext)<br/>

### **Send(TSplit)**

```csharp
public Task Send(TSplit context)
```

#### Parameters

`context` TSplit<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
