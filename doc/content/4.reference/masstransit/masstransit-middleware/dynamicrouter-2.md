---

title: DynamicRouter<TContext, TKey>

---

# DynamicRouter\<TContext, TKey\>

Namespace: MassTransit.Middleware

```csharp
public class DynamicRouter<TContext, TKey> : IDynamicRouter<TContext, TKey>, IDynamicRouter<TContext>, IPipe<TContext>, IProbeSite, IPipeConnector, IFilterObserverConnector, IKeyPipeConnector<TKey>
```

#### Type Parameters

`TContext`<br/>

`TKey`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DynamicRouter\<TContext, TKey\>](../masstransit-middleware/dynamicrouter-2)<br/>
Implements [IDynamicRouter\<TContext, TKey\>](../masstransit-middleware/idynamicrouter-2), [IDynamicRouter\<TContext\>](../masstransit-middleware/idynamicrouter-1), [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector](../masstransit-middleware/ipipeconnector), [IFilterObserverConnector](../../masstransit-abstractions/masstransit/ifilterobserverconnector), [IKeyPipeConnector\<TKey\>](../masstransit-middleware/ikeypipeconnector-1)

## Constructors

### **DynamicRouter(IPipeContextConverterFactory\<TContext\>, KeyAccessor\<TContext, TKey\>)**

```csharp
public DynamicRouter(IPipeContextConverterFactory<TContext> converterFactory, KeyAccessor<TContext, TKey> keyAccessor)
```

#### Parameters

`converterFactory` [IPipeContextConverterFactory\<TContext\>](../masstransit-middleware/ipipecontextconverterfactory-1)<br/>

`keyAccessor` [KeyAccessor\<TContext, TKey\>](../masstransit-middleware/keyaccessor-2)<br/>

## Methods

### **ConnectPipe\<T\>(IPipe\<T\>)**

```csharp
public ConnectHandle ConnectPipe<T>(IPipe<T> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<T\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

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
