---

title: ISagaRepository<TSaga>

---

# ISagaRepository\<TSaga\>

Namespace: MassTransit

A saga repository is used by the service bus to dispatch messages to sagas

```csharp
public interface ISagaRepository<TSaga> : IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Send\<T\>(ConsumeContext\<T\>, ISagaPolicy\<TSaga, T\>, IPipe\<SagaConsumeContext\<TSaga, T\>\>)**

Send the message to the saga repository where the context.CorrelationId has the CorrelationId
 of the saga instance.

```csharp
Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>
The message consume context

`policy` [ISagaPolicy\<TSaga, T\>](../masstransit/isagapolicy-2)<br/>
The saga policy for the message

`next` [IPipe\<SagaConsumeContext\<TSaga, T\>\>](../masstransit/ipipe-1)<br/>
The saga consume pipe

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendQuery\<T\>(ConsumeContext\<T\>, ISagaQuery\<TSaga\>, ISagaPolicy\<TSaga, T\>, IPipe\<SagaConsumeContext\<TSaga, T\>\>)**

Send the message to the saga repository where the query is used to find matching saga instances,
 which are invoked concurrently.

```csharp
Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>
The saga query consume context

`query` [ISagaQuery\<TSaga\>](../masstransit/isagaquery-1)<br/>

`policy` [ISagaPolicy\<TSaga, T\>](../masstransit/isagapolicy-2)<br/>
The saga policy for the message

`next` [IPipe\<SagaConsumeContext\<TSaga, T\>\>](../masstransit/ipipe-1)<br/>
The saga consume pipe

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
