---

title: IndexedSagaDictionary<TSaga>

---

# IndexedSagaDictionary\<TSaga\>

Namespace: MassTransit.Saga

```csharp
public class IndexedSagaDictionary<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [IndexedSagaDictionary\<TSaga\>](../masstransit-saga/indexedsagadictionary-1)

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

### **IndexedSagaDictionary()**

```csharp
public IndexedSagaDictionary()
```

## Methods

### **MarkInUse(CancellationToken)**

```csharp
public Task MarkInUse(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Release()**

```csharp
public void Release()
```

### **Add(SagaInstance\<TSaga\>)**

```csharp
public void Add(SagaInstance<TSaga> instance)
```

#### Parameters

`instance` [SagaInstance\<TSaga\>](../masstransit-saga/sagainstance-1)<br/>

### **Remove(SagaInstance\<TSaga\>)**

```csharp
public void Remove(SagaInstance<TSaga> item)
```

#### Parameters

`item` [SagaInstance\<TSaga\>](../masstransit-saga/sagainstance-1)<br/>

### **Where(ISagaQuery\<TSaga\>)**

```csharp
public IEnumerable<SagaInstance<TSaga>> Where(ISagaQuery<TSaga> query)
```

#### Parameters

`query` [ISagaQuery\<TSaga\>](../../masstransit-abstractions/masstransit/isagaquery-1)<br/>

#### Returns

[IEnumerable\<SagaInstance\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Select\<TResult\>(Func\<TSaga, TResult\>)**

```csharp
public IEnumerable<TResult> Select<TResult>(Func<TSaga, TResult> transformer)
```

#### Type Parameters

`TResult`<br/>

#### Parameters

`transformer` [Func\<TSaga, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IEnumerable\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
