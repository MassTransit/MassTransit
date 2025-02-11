---

title: PipeContextSourceBindFilter<TLeft, TRight>

---

# PipeContextSourceBindFilter\<TLeft, TRight\>

Namespace: MassTransit.Middleware

Binds a context to the pipe using a .

```csharp
public class PipeContextSourceBindFilter<TLeft, TRight> : IFilter<TLeft>, IProbeSite
```

#### Type Parameters

`TLeft`<br/>

`TRight`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PipeContextSourceBindFilter\<TLeft, TRight\>](../masstransit-middleware/pipecontextsourcebindfilter-2)<br/>
Implements [IFilter\<TLeft\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **PipeContextSourceBindFilter(IPipe\<BindContext\<TLeft, TRight\>\>, IPipeContextSource\<TRight, TLeft\>)**

```csharp
public PipeContextSourceBindFilter(IPipe<BindContext<TLeft, TRight>> output, IPipeContextSource<TRight, TLeft> source)
```

#### Parameters

`output` [IPipe\<BindContext\<TLeft, TRight\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`source` [IPipeContextSource\<TRight, TLeft\>](../../masstransit-abstractions/masstransit/ipipecontextsource-2)<br/>

## Methods

### **Send(TLeft, IPipe\<TLeft\>)**

```csharp
public Task Send(TLeft context, IPipe<TLeft> next)
```

#### Parameters

`context` TLeft<br/>

`next` [IPipe\<TLeft\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
