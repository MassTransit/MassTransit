---

title: KeyFilter<TContext, TKey>

---

# KeyFilter\<TContext, TKey\>

Namespace: MassTransit.Middleware

Handles the registration of requests and connecting them to the consume pipe

```csharp
public class KeyFilter<TContext, TKey> : IFilter<TContext>, IProbeSite, IKeyPipeConnector<TKey>
```

#### Type Parameters

`TContext`<br/>

`TKey`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [KeyFilter\<TContext, TKey\>](../masstransit-middleware/keyfilter-2)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IKeyPipeConnector\<TKey\>](../masstransit-middleware/ikeypipeconnector-1)

## Constructors

### **KeyFilter(KeyAccessor\<TContext, TKey\>)**

```csharp
public KeyFilter(KeyAccessor<TContext, TKey> keyAccessor)
```

#### Parameters

`keyAccessor` [KeyAccessor\<TContext, TKey\>](../masstransit-middleware/keyaccessor-2)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(TContext, IPipe\<TContext\>)**

```csharp
public Task Send(TContext context, IPipe<TContext> next)
```

#### Parameters

`context` TContext<br/>

`next` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

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
