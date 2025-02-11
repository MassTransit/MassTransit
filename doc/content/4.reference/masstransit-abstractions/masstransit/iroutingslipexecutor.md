---

title: IRoutingSlipExecutor

---

# IRoutingSlipExecutor

Namespace: MassTransit

```csharp
public interface IRoutingSlipExecutor
```

## Methods

### **Execute(RoutingSlip, CancellationToken)**

Execute a routing slip

```csharp
Task Execute(RoutingSlip routingSlip, CancellationToken cancellationToken)
```

#### Parameters

`routingSlip` [RoutingSlip](../masstransit-courier-contracts/routingslip)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
