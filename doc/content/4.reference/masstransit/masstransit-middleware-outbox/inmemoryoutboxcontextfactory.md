---

title: InMemoryOutboxContextFactory

---

# InMemoryOutboxContextFactory

Namespace: MassTransit.Middleware.Outbox

```csharp
public class InMemoryOutboxContextFactory : IOutboxContextFactory<InMemoryOutboxMessageRepository>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryOutboxContextFactory](../masstransit-middleware-outbox/inmemoryoutboxcontextfactory)<br/>
Implements [IOutboxContextFactory\<InMemoryOutboxMessageRepository\>](../masstransit-middleware/ioutboxcontextfactory-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **InMemoryOutboxContextFactory(InMemoryOutboxMessageRepository, IServiceProvider)**

```csharp
public InMemoryOutboxContextFactory(InMemoryOutboxMessageRepository messageRepository, IServiceProvider provider)
```

#### Parameters

`messageRepository` [InMemoryOutboxMessageRepository](../masstransit-middleware-outbox/inmemoryoutboxmessagerepository)<br/>

`provider` IServiceProvider<br/>

## Methods

### **Send\<T\>(ConsumeContext\<T\>, OutboxConsumeOptions, IPipe\<OutboxConsumeContext\<T\>\>)**

```csharp
public Task Send<T>(ConsumeContext<T> context, OutboxConsumeOptions options, IPipe<OutboxConsumeContext<T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`options` [OutboxConsumeOptions](../masstransit-middleware/outboxconsumeoptions)<br/>

`next` [IPipe\<OutboxConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
