---

title: ISagaQuery<TSaga>

---

# ISagaQuery\<TSaga\>

Namespace: MassTransit

A saga query is used when a LINQ expression is accepted to query
 the saga repository storage to get zero or more saga instances

```csharp
public interface ISagaQuery<TSaga>
```

#### Type Parameters

`TSaga`<br/>

## Properties

### **FilterExpression**

The query expression that returns true if the saga
 matches the query.

```csharp
public abstract Expression<Func<TSaga, bool>> FilterExpression { get; }
```

#### Property Value

Expression\<Func\<TSaga, Boolean\>\><br/>

## Methods

### **GetFilter()**

Compiles a function that can be used to programatically
 compare a saga instance to the filter expression.

```csharp
Func<TSaga, bool> GetFilter()
```

#### Returns

[Func\<TSaga, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
