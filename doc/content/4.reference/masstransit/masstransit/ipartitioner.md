---

title: IPartitioner

---

# IPartitioner

Namespace: MassTransit

```csharp
public interface IPartitioner : IAsyncDisposable, IProbeSite
```

Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **GetPartitioner\<T\>(PartitionKeyProvider\<T\>)**

```csharp
IPartitioner<T> GetPartitioner<T>(PartitionKeyProvider<T> keyProvider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`keyProvider` [PartitionKeyProvider\<T\>](../masstransit/partitionkeyprovider-1)<br/>

#### Returns

[IPartitioner\<T\>](../masstransit/ipartitioner-1)<br/>
