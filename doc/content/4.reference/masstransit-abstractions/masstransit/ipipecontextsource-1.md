---

title: IPipeContextSource<TContext>

---

# IPipeContextSource\<TContext\>

Namespace: MassTransit

A source provides the context which is sent to the specified pipe.

```csharp
public interface IPipeContextSource<TContext> : IProbeSite
```

#### Type Parameters

`TContext`<br/>
The pipe context type

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Send(IPipe\<TContext\>, CancellationToken)**

Send a context from the source through the specified pipe

```csharp
Task Send(IPipe<TContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`pipe` [IPipe\<TContext\>](../masstransit/ipipe-1)<br/>
The destination pipe

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
The cancellationToken, which should be included in the context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
