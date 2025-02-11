---

title: RoutingSlipRequestProxy<TRequest>

---

# RoutingSlipRequestProxy\<TRequest\>

Namespace: MassTransit.Courier

```csharp
public abstract class RoutingSlipRequestProxy<TRequest> : IConsumer<TRequest>, IConsumer
```

#### Type Parameters

`TRequest`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipRequestProxy\<TRequest\>](../masstransit-courier/routingsliprequestproxy-1)<br/>
Implements [IConsumer\<TRequest\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer)

## Methods

### **Consume(ConsumeContext\<TRequest\>)**

```csharp
public Task Consume(ConsumeContext<TRequest> context)
```

#### Parameters

`context` [ConsumeContext\<TRequest\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **BuildRoutingSlip(RoutingSlipBuilder, ConsumeContext\<TRequest\>)**

```csharp
protected abstract Task BuildRoutingSlip(RoutingSlipBuilder builder, ConsumeContext<TRequest> request)
```

#### Parameters

`builder` [RoutingSlipBuilder](../masstransit/routingslipbuilder)<br/>

`request` [ConsumeContext\<TRequest\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **GetResponseEndpointAddress(ConsumeContext\<TRequest\>)**

By default, returns the input address of the request consumer which assumes the response consumer is on the same receive endpoint.
 Override to specify the endpoint address of the response consumer if it is configured on a separate receive endpoint.

```csharp
protected Uri GetResponseEndpointAddress(ConsumeContext<TRequest> context)
```

#### Parameters

`context` [ConsumeContext\<TRequest\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

Uri<br/>
