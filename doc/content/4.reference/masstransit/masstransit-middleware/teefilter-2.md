---

title: TeeFilter<TContext, TKey>

---

# TeeFilter\<TContext, TKey\>

Namespace: MassTransit.Middleware

Connects multiple output pipes to a single input pipe

```csharp
public class TeeFilter<TContext, TKey> : TeeFilter<TContext>, ITeeFilter<TContext>, IFilter<TContext>, IProbeSite, IPipeConnector<TContext>, ITeeFilter<TContext, TKey>, IKeyPipeConnector<TKey>
```

#### Type Parameters

`TContext`<br/>

`TKey`<br/>
The key type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TeeFilter\<TContext\>](../masstransit-middleware/teefilter-1) → [TeeFilter\<TContext, TKey\>](../masstransit-middleware/teefilter-2)<br/>
Implements [ITeeFilter\<TContext\>](../masstransit-middleware/iteefilter-1), [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector\<TContext\>](../masstransit-middleware/ipipeconnector-1), [ITeeFilter\<TContext, TKey\>](../masstransit-middleware/iteefilter-2), [IKeyPipeConnector\<TKey\>](../masstransit-middleware/ikeypipeconnector-1)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **TeeFilter(KeyAccessor\<TContext, TKey\>)**

```csharp
public TeeFilter(KeyAccessor<TContext, TKey> keyAccessor)
```

#### Parameters

`keyAccessor` [KeyAccessor\<TContext, TKey\>](../masstransit-middleware/keyaccessor-2)<br/>

## Methods

### **ConnectPipe\<T\>(TKey, IPipe\<T\>)**

```csharp
public ConnectHandle ConnectPipe<T>(TKey key, IPipe<T> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`key` TKey<br/>

`pipe` [IPipe\<T\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
