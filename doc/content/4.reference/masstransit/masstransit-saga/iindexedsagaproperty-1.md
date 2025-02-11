---

title: IIndexedSagaProperty<TSaga>

---

# IIndexedSagaProperty\<TSaga\>

Namespace: MassTransit.Saga

For the in-memory saga repository, this maintains an index of saga properties
 for fast searching

```csharp
public interface IIndexedSagaProperty<TSaga>
```

#### Type Parameters

`TSaga`<br/>
The saga type

## Properties

### **Item**

```csharp
public abstract SagaInstance<TSaga> Item { get; }
```

#### Property Value

[SagaInstance\<TSaga\>](../masstransit-saga/sagainstance-1)<br/>

### **Count**

```csharp
public abstract int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **Add(SagaInstance\<TSaga\>)**

Adds a new saga to the index

```csharp
void Add(SagaInstance<TSaga> newItem)
```

#### Parameters

`newItem` [SagaInstance\<TSaga\>](../masstransit-saga/sagainstance-1)<br/>

### **Remove(SagaInstance\<TSaga\>)**

Removes a saga from the index

```csharp
void Remove(SagaInstance<TSaga> item)
```

#### Parameters

`item` [SagaInstance\<TSaga\>](../masstransit-saga/sagainstance-1)<br/>

### **Where(Func\<TSaga, Boolean\>)**

Returns sagas matching the filter function

```csharp
IEnumerable<SagaInstance<TSaga>> Where(Func<TSaga, bool> filter)
```

#### Parameters

`filter` [Func\<TSaga, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IEnumerable\<SagaInstance\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Where(Object, Func\<TSaga, Boolean\>)**

Returns sagas matching the filter function where the key also matches

```csharp
IEnumerable<SagaInstance<TSaga>> Where(object key, Func<TSaga, bool> filter)
```

#### Parameters

`key` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`filter` [Func\<TSaga, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IEnumerable\<SagaInstance\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Select\<TResult\>(Func\<TSaga, TResult\>)**

Selects sagas from the index, running the transformation function and returning the output type

```csharp
IEnumerable<TResult> Select<TResult>(Func<TSaga, TResult> transformer)
```

#### Type Parameters

`TResult`<br/>

#### Parameters

`transformer` [Func\<TSaga, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IEnumerable\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
