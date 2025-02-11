---

title: IDynamicRouter<TContext, TKey>

---

# IDynamicRouter\<TContext, TKey\>

Namespace: MassTransit.Middleware

A dynamic router is a pipe on which additional pipes can be connected and context is
 routed through the pipe based upon the output requirements of the connected pipes. It is built
 around the dynamic filter, which is the central point of the router.

```csharp
public interface IDynamicRouter<TContext, TKey> : IDynamicRouter<TContext>, IPipe<TContext>, IProbeSite, IPipeConnector, IFilterObserverConnector, IKeyPipeConnector<TKey>
```

#### Type Parameters

`TContext`<br/>

`TKey`<br/>

Implements [IDynamicRouter\<TContext\>](../masstransit-middleware/idynamicrouter-1), [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector](../masstransit-middleware/ipipeconnector), [IFilterObserverConnector](../../masstransit-abstractions/masstransit/ifilterobserverconnector), [IKeyPipeConnector\<TKey\>](../masstransit-middleware/ikeypipeconnector-1)
