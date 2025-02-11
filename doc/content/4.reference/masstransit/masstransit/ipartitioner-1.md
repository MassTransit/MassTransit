---

title: IPartitioner<TContext>

---

# IPartitioner\<TContext\>

Namespace: MassTransit

```csharp
public interface IPartitioner<TContext> : IProbeSite
```

#### Type Parameters

`TContext`<br/>

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **Send(TContext, IPipe\<TContext\>)**

Sends the context through the partitioner

```csharp
Task Send(TContext context, IPipe<TContext> next)
```

#### Parameters

`context` TContext<br/>
The context

`next` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
The next pipe

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
