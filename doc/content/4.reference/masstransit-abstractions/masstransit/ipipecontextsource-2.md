---

title: IPipeContextSource<TContext, TInput>

---

# IPipeContextSource\<TContext, TInput\>

Namespace: MassTransit

A source which provides the context using the input context to select the appropriate source.

```csharp
public interface IPipeContextSource<TContext, TInput> : IProbeSite
```

#### Type Parameters

`TContext`<br/>
The output context type

`TInput`<br/>
The input context type

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Send(TInput, IPipe\<TContext\>)**

Send a context from the source through the specified pipe, using the input context to select the proper source.

```csharp
Task Send(TInput context, IPipe<TContext> pipe)
```

#### Parameters

`context` TInput<br/>

`pipe` [IPipe\<TContext\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
