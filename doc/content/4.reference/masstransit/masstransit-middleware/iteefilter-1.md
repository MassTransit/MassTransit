---

title: ITeeFilter<TContext>

---

# ITeeFilter\<TContext\>

Namespace: MassTransit.Middleware

```csharp
public interface ITeeFilter<TContext> : IFilter<TContext>, IProbeSite, IPipeConnector<TContext>
```

#### Type Parameters

`TContext`<br/>

Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector\<TContext\>](../masstransit-middleware/ipipeconnector-1)

## Properties

### **Count**

```csharp
public abstract int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
