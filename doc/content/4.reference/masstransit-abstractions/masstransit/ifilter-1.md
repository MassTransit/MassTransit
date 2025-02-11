---

title: IFilter<TContext>

---

# IFilter\<TContext\>

Namespace: MassTransit

A filter is a functional node in a pipeline, connected by pipes to
 other filters.

```csharp
public interface IFilter<TContext> : IProbeSite
```

#### Type Parameters

`TContext`<br/>
The pipe context type

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Send(TContext, IPipe\<TContext\>)**

Sends a context to a filter, such that it can be processed and then passed to the
 specified output pipe for further processing.

```csharp
Task Send(TContext context, IPipe<TContext> next)
```

#### Parameters

`context` TContext<br/>
The pipe context type

`next` [IPipe\<TContext\>](../masstransit/ipipe-1)<br/>
The next pipe in the pipeline

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
An awaitable Task
