---

title: Partitioner

---

# Partitioner

Namespace: MassTransit.Middleware

```csharp
public class Partitioner : IPartitioner, IAsyncDisposable, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Partitioner](../masstransit-middleware/partitioner)<br/>
Implements [IPartitioner](../masstransit/ipartitioner), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **Partitioner(Int32, IHashGenerator)**

```csharp
public Partitioner(int partitionCount, IHashGenerator hashGenerator)
```

#### Parameters

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`hashGenerator` [IHashGenerator](../masstransit-middleware/ihashgenerator)<br/>

## Methods

### **GetPartitioner\<T\>(PartitionKeyProvider\<T\>)**

```csharp
public IPartitioner<T> GetPartitioner<T>(PartitionKeyProvider<T> keyProvider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`keyProvider` [PartitionKeyProvider\<T\>](../masstransit/partitionkeyprovider-1)<br/>

#### Returns

[IPartitioner\<T\>](../masstransit/ipartitioner-1)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>
