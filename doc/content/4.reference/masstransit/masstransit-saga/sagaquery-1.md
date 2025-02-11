---

title: SagaQuery<TSaga>

---

# SagaQuery\<TSaga\>

Namespace: MassTransit.Saga

```csharp
public class SagaQuery<TSaga> : ISagaQuery<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaQuery\<TSaga\>](../masstransit-saga/sagaquery-1)<br/>
Implements [ISagaQuery\<TSaga\>](../../masstransit-abstractions/masstransit/isagaquery-1)

## Properties

### **FilterExpression**

```csharp
public Expression<Func<TSaga, bool>> FilterExpression { get; }
```

#### Property Value

Expression\<Func\<TSaga, Boolean\>\><br/>

## Constructors

### **SagaQuery(Expression\<Func\<TSaga, Boolean\>\>)**

```csharp
public SagaQuery(Expression<Func<TSaga, bool>> filterExpression)
```

#### Parameters

`filterExpression` Expression\<Func\<TSaga, Boolean\>\><br/>

## Methods

### **GetFilter()**

```csharp
public Func<TSaga, bool> GetFilter()
```

#### Returns

[Func\<TSaga, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
