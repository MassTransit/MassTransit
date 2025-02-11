---

title: DynamicRouter<TContext>

---

# DynamicRouter\<TContext\>

Namespace: MassTransit.Middleware

A dynamic router is a pipe on which additional pipes can be connected and context is
 routed through the pipe based upon the output requirements of the connected pipes. It is built
 around the dynamic filter, which is the central point of the router.

```csharp
public class DynamicRouter<TContext> : IDynamicRouter<TContext>, IPipe<TContext>, IProbeSite, IPipeConnector, IFilterObserverConnector
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DynamicRouter\<TContext\>](../masstransit-middleware/dynamicrouter-1)<br/>
Implements [IDynamicRouter\<TContext\>](../masstransit-middleware/idynamicrouter-1), [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector](../masstransit-middleware/ipipeconnector), [IFilterObserverConnector](../../masstransit-abstractions/masstransit/ifilterobserverconnector)

## Constructors

### **DynamicRouter(IPipeContextConverterFactory\<TContext\>)**

```csharp
public DynamicRouter(IPipeContextConverterFactory<TContext> converterFactory)
```

#### Parameters

`converterFactory` [IPipeContextConverterFactory\<TContext\>](../masstransit-middleware/ipipecontextconverterfactory-1)<br/>

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
