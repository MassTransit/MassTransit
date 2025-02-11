---

title: ISagaQueryFactory<TSaga, TMessage>

---

# ISagaQueryFactory\<TSaga, TMessage\>

Namespace: MassTransit

Used to create a saga query from the message consume context

```csharp
public interface ISagaQueryFactory<TSaga, TMessage> : IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **TryCreateQuery(ConsumeContext\<TMessage\>, ISagaQuery\<TSaga\>)**

Creates a saga query from the specified message context

```csharp
bool TryCreateQuery(ConsumeContext<TMessage> context, out ISagaQuery<TSaga> query)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../masstransit/consumecontext-1)<br/>
The message context

`query` [ISagaQuery\<TSaga\>](../masstransit/isagaquery-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
