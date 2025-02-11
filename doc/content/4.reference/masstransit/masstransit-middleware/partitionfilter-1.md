---

title: PartitionFilter<TContext>

---

# PartitionFilter\<TContext\>

Namespace: MassTransit.Middleware

```csharp
public class PartitionFilter<TContext> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PartitionFilter\<TContext\>](../masstransit-middleware/partitionfilter-1)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **PartitionFilter(PartitionKeyProvider\<TContext\>, IPartitioner)**

```csharp
public PartitionFilter(PartitionKeyProvider<TContext> keyProvider, IPartitioner partitioner)
```

#### Parameters

`keyProvider` [PartitionKeyProvider\<TContext\>](../masstransit/partitionkeyprovider-1)<br/>

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>
