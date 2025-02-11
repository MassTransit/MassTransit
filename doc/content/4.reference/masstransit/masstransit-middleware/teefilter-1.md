---

title: TeeFilter<TContext>

---

# TeeFilter\<TContext\>

Namespace: MassTransit.Middleware

Connects multiple output pipes to a single input pipe

```csharp
public class TeeFilter<TContext> : ITeeFilter<TContext>, IFilter<TContext>, IProbeSite, IPipeConnector<TContext>
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TeeFilter\<TContext\>](../masstransit-middleware/teefilter-1)<br/>
Implements [ITeeFilter\<TContext\>](../masstransit-middleware/iteefilter-1), [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector\<TContext\>](../masstransit-middleware/ipipeconnector-1)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **TeeFilter()**

```csharp
public TeeFilter()
```

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(TContext, IPipe\<TContext\>)**

```csharp
public Task Send(TContext context, IPipe<TContext> next)
```

#### Parameters

`context` TContext<br/>

`next` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConnectPipe(IPipe\<TContext\>)**

```csharp
public ConnectHandle ConnectPipe(IPipe<TContext> pipe)
```

#### Parameters

`pipe` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
