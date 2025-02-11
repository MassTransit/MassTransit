---

title: IQuerySagaRepository<TSaga>

---

# IQuerySagaRepository\<TSaga\>

Namespace: MassTransit

```csharp
public interface IQuerySagaRepository<TSaga> : IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Find(ISagaQuery\<TSaga\>)**

```csharp
Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
```

#### Parameters

`query` [ISagaQuery\<TSaga\>](../masstransit/isagaquery-1)<br/>

#### Returns

[Task\<IEnumerable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
