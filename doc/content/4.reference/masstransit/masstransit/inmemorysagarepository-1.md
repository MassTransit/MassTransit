---

title: InMemorySagaRepository<TSaga>

---

# InMemorySagaRepository\<TSaga\>

Namespace: MassTransit

```csharp
public class InMemorySagaRepository<TSaga> : ISagaRepository<TSaga>, IProbeSite, IQuerySagaRepository<TSaga>, ILoadSagaRepository<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemorySagaRepository\<TSaga\>](../masstransit/inmemorysagarepository-1)<br/>
Implements [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IQuerySagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iquerysagarepository-1), [ILoadSagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iloadsagarepository-1)

## Properties

### **Item**

```csharp
public SagaInstance<TSaga> Item { get; }
```

#### Property Value

[SagaInstance\<TSaga\>](../masstransit-saga/sagainstance-1)<br/>

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **InMemorySagaRepository()**

```csharp
public InMemorySagaRepository()
```

## Methods

### **Load(Guid)**

```csharp
public Task<TSaga> Load(Guid correlationId)
```

#### Parameters

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Task\<TSaga\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Find(ISagaQuery\<TSaga\>)**

```csharp
public Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
```

#### Parameters

`query` [ISagaQuery\<TSaga\>](../../masstransit-abstractions/masstransit/isagaquery-1)<br/>

#### Returns

[Task\<IEnumerable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
