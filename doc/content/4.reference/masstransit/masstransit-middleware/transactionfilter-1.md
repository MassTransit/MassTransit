---

title: TransactionFilter<T>

---

# TransactionFilter\<T\>

Namespace: MassTransit.Middleware

```csharp
public class TransactionFilter<T> : IFilter<T>, IProbeSite
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransactionFilter\<T\>](../masstransit-middleware/transactionfilter-1)<br/>
Implements [IFilter\<T\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **TransactionFilter(IsolationLevel, TimeSpan)**

```csharp
public TransactionFilter(IsolationLevel isolationLevel, TimeSpan timeout)
```

#### Parameters

`isolationLevel` IsolationLevel<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Methods

### **Send(T, IPipe\<T\>)**

```csharp
public Task Send(T context, IPipe<T> next)
```

#### Parameters

`context` T<br/>

`next` [IPipe\<T\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
