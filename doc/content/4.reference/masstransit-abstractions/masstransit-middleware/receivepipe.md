---

title: ReceivePipe

---

# ReceivePipe

Namespace: MassTransit.Middleware

```csharp
public class ReceivePipe : IReceivePipe, IPipe<ReceiveContext>, IProbeSite, IConsumePipeConnector, IRequestPipeConnector, IConsumeMessageObserverConnector, IConsumeObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceivePipe](../masstransit-middleware/receivepipe)<br/>
Implements [IReceivePipe](../masstransit-transports/ireceivepipe), [IPipe\<ReceiveContext\>](../masstransit/ipipe-1), [IProbeSite](../masstransit/iprobesite), [IConsumePipeConnector](../masstransit/iconsumepipeconnector), [IRequestPipeConnector](../masstransit/irequestpipeconnector), [IConsumeMessageObserverConnector](../masstransit/iconsumemessageobserverconnector), [IConsumeObserverConnector](../masstransit/iconsumeobserverconnector)

## Properties

### **Connected**

```csharp
public Task Connected { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

## Constructors

### **ReceivePipe(IPipe\<ReceiveContext\>, IConsumePipe)**

```csharp
public ReceivePipe(IPipe<ReceiveContext> receivePipe, IConsumePipe consumePipe)
```

#### Parameters

`receivePipe` [IPipe\<ReceiveContext\>](../masstransit/ipipe-1)<br/>

`consumePipe` [IConsumePipe](../masstransit-transports/iconsumepipe)<br/>
