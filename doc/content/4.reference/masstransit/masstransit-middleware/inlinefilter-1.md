---

title: InlineFilter<TContext>

---

# InlineFilter\<TContext\>

Namespace: MassTransit.Middleware

```csharp
public class InlineFilter<TContext> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InlineFilter\<TContext\>](../masstransit-middleware/inlinefilter-1)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **InlineFilter(InlineFilterMethod\<TContext\>)**

```csharp
public InlineFilter(InlineFilterMethod<TContext> filterMethod)
```

#### Parameters

`filterMethod` [InlineFilterMethod\<TContext\>](../masstransit/inlinefiltermethod-1)<br/>

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
