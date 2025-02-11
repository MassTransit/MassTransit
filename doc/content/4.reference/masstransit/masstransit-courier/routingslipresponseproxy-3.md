---

title: RoutingSlipResponseProxy<TRequest, TResponse, TFault>

---

# RoutingSlipResponseProxy\<TRequest, TResponse, TFault\>

Namespace: MassTransit.Courier

```csharp
public abstract class RoutingSlipResponseProxy<TRequest, TResponse, TFault> : IConsumer<RoutingSlipCompleted>, IConsumer, IConsumer<RoutingSlipFaulted>
```

#### Type Parameters

`TRequest`<br/>

`TResponse`<br/>

`TFault`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipResponseProxy\<TRequest, TResponse, TFault\>](../masstransit-courier/routingslipresponseproxy-3)<br/>
Implements [IConsumer\<RoutingSlipCompleted\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer), [IConsumer\<RoutingSlipFaulted\>](../../masstransit-abstractions/masstransit/iconsumer-1)

## Methods

### **Consume(ConsumeContext\<RoutingSlipCompleted\>)**

```csharp
public Task Consume(ConsumeContext<RoutingSlipCompleted> context)
```

#### Parameters

`context` [ConsumeContext\<RoutingSlipCompleted\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Consume(ConsumeContext\<RoutingSlipFaulted\>)**

```csharp
public Task Consume(ConsumeContext<RoutingSlipFaulted> context)
```

#### Parameters

`context` [ConsumeContext\<RoutingSlipFaulted\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CreateResponseMessage(ConsumeContext\<RoutingSlipCompleted\>, TRequest)**

```csharp
protected abstract Task<TResponse> CreateResponseMessage(ConsumeContext<RoutingSlipCompleted> context, TRequest request)
```

#### Parameters

`context` [ConsumeContext\<RoutingSlipCompleted\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`request` TRequest<br/>

#### Returns

[Task\<TResponse\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CreateFaultedResponseMessage(ConsumeContext\<RoutingSlipFaulted\>, TRequest, Guid)**

```csharp
protected abstract Task<TFault> CreateFaultedResponseMessage(ConsumeContext<RoutingSlipFaulted> context, TRequest request, Guid requestId)
```

#### Parameters

`context` [ConsumeContext\<RoutingSlipFaulted\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`request` TRequest<br/>

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Task\<TFault\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
