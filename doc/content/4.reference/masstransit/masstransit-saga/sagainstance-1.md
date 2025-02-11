---

title: SagaInstance<TSaga>

---

# SagaInstance\<TSaga\>

Namespace: MassTransit.Saga

```csharp
public class SagaInstance<TSaga> : IEquatable<SagaInstance<TSaga>>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaInstance\<TSaga\>](../masstransit-saga/sagainstance-1)<br/>
Implements [IEquatable\<SagaInstance\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Instance**

```csharp
public TSaga Instance { get; }
```

#### Property Value

TSaga<br/>

### **IsRemoved**

```csharp
public bool IsRemoved { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **SagaInstance(TSaga)**

```csharp
public SagaInstance(TSaga instance)
```

#### Parameters

`instance` TSaga<br/>

## Methods

### **Equals(SagaInstance\<TSaga\>)**

```csharp
public bool Equals(SagaInstance<TSaga> other)
```

#### Parameters

`other` [SagaInstance\<TSaga\>](../masstransit-saga/sagainstance-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

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

### **Remove()**

```csharp
public void Remove()
```
