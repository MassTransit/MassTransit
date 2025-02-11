---

title: RoutingSlipExecutor

---

# RoutingSlipExecutor

Namespace: MassTransit.Courier

```csharp
public class RoutingSlipExecutor : IRoutingSlipExecutor
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipExecutor](../masstransit-courier/routingslipexecutor)<br/>
Implements [IRoutingSlipExecutor](../masstransit/iroutingslipexecutor)

## Constructors

### **RoutingSlipExecutor(ISendEndpointProvider, IPublishEndpoint)**

```csharp
public RoutingSlipExecutor(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
```

#### Parameters

`sendEndpointProvider` [ISendEndpointProvider](../masstransit/isendendpointprovider)<br/>

`publishEndpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>

## Methods

### **Execute(RoutingSlip, CancellationToken)**

```csharp
public Task Execute(RoutingSlip routingSlip, CancellationToken cancellationToken)
```

#### Parameters

`routingSlip` [RoutingSlip](../masstransit-courier-contracts/routingslip)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
