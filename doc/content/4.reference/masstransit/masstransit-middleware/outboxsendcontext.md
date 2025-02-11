---

title: OutboxSendContext

---

# OutboxSendContext

Namespace: MassTransit.Middleware

Used by the new outbox construct

```csharp
public interface OutboxSendContext : IServiceProvider
```

Implements IServiceProvider

## Methods

### **AddSend\<T\>(SendContext\<T\>)**

```csharp
Task AddSend<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
