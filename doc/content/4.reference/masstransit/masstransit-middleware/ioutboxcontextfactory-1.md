---

title: IOutboxContextFactory<TContext>

---

# IOutboxContextFactory\<TContext\>

Namespace: MassTransit.Middleware

```csharp
public interface IOutboxContextFactory<TContext> : IProbeSite
```

#### Type Parameters

`TContext`<br/>

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **Send\<T\>(ConsumeContext\<T\>, OutboxConsumeOptions, IPipe\<OutboxConsumeContext\<T\>\>)**

```csharp
Task Send<T>(ConsumeContext<T> context, OutboxConsumeOptions options, IPipe<OutboxConsumeContext<T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`options` [OutboxConsumeOptions](../masstransit-middleware/outboxconsumeoptions)<br/>

`next` [IPipe\<OutboxConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
