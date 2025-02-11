---

title: RoutingSlipResponseProxy<TRequest, TResponse>

---

# RoutingSlipResponseProxy\<TRequest, TResponse\>

Namespace: MassTransit.Courier

```csharp
public abstract class RoutingSlipResponseProxy<TRequest, TResponse> : RoutingSlipResponseProxy<TRequest, TResponse, Fault<TRequest>>, IConsumer<RoutingSlipCompleted>, IConsumer, IConsumer<RoutingSlipFaulted>
```

#### Type Parameters

`TRequest`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipResponseProxy\<TRequest, TResponse, Fault\<TRequest\>\>](../masstransit-courier/routingslipresponseproxy-3) → [RoutingSlipResponseProxy\<TRequest, TResponse\>](../masstransit-courier/routingslipresponseproxy-2)<br/>
Implements [IConsumer\<RoutingSlipCompleted\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer), [IConsumer\<RoutingSlipFaulted\>](../../masstransit-abstractions/masstransit/iconsumer-1)

## Methods

### **CreateFaultedResponseMessage(ConsumeContext\<RoutingSlipFaulted\>, TRequest, Guid)**

```csharp
protected Task<Fault<TRequest>> CreateFaultedResponseMessage(ConsumeContext<RoutingSlipFaulted> context, TRequest request, Guid requestId)
```

#### Parameters

`context` [ConsumeContext\<RoutingSlipFaulted\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`request` TRequest<br/>

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Task\<Fault\<TRequest\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
