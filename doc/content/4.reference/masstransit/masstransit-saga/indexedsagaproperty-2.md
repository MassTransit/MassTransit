---

title: IndexedSagaProperty<TSaga, TProperty>

---

# IndexedSagaProperty\<TSaga, TProperty\>

Namespace: MassTransit.Saga

A dictionary index of the sagas

```csharp
public class IndexedSagaProperty<TSaga, TProperty> : IIndexedSagaProperty<TSaga>
```

#### Type Parameters

`TSaga`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [IndexedSagaProperty\<TSaga, TProperty\>](../masstransit-saga/indexedsagaproperty-2)<br/>
Implements [IIndexedSagaProperty\<TSaga\>](../masstransit-saga/iindexedsagaproperty-1)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Item**

```csharp
public SagaInstance<TSaga> Item { get; }
```

#### Property Value

[SagaInstance\<TSaga\>](../masstransit-saga/sagainstance-1)<br/>

## Constructors

### **IndexedSagaProperty(PropertyInfo)**

Creates an index for the specified property of a saga

```csharp
public IndexedSagaProperty(PropertyInfo propertyInfo)
```

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Add(SagaInstance\<TSaga\>)**

```csharp
public void Add(SagaInstance<TSaga> newItem)
```

#### Parameters

`newItem` [SagaInstance\<TSaga\>](../masstransit-saga/sagainstance-1)<br/>

### **Remove(SagaInstance\<TSaga\>)**

```csharp
public void Remove(SagaInstance<TSaga> instance)
```

#### Parameters

`instance` [SagaInstance\<TSaga\>](../masstransit-saga/sagainstance-1)<br/>

### **Where(Func\<TSaga, Boolean\>)**

```csharp
public IEnumerable<SagaInstance<TSaga>> Where(Func<TSaga, bool> filter)
```

#### Parameters

`filter` [Func\<TSaga, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IEnumerable\<SagaInstance\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Where(Object, Func\<TSaga, Boolean\>)**

```csharp
public IEnumerable<SagaInstance<TSaga>> Where(object key, Func<TSaga, bool> filter)
```

#### Parameters

`key` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`filter` [Func\<TSaga, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
