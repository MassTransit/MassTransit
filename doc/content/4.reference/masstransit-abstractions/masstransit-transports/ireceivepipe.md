---

title: IReceivePipe

---

# IReceivePipe

Namespace: MassTransit.Transports

```csharp
public interface IReceivePipe : IPipe<ReceiveContext>, IProbeSite, IConsumePipeConnector, IRequestPipeConnector, IConsumeMessageObserverConnector, IConsumeObserverConnector
```

Implements [IPipe\<ReceiveContext\>](../masstransit/ipipe-1), [IProbeSite](../masstransit/iprobesite), [IConsumePipeConnector](../masstransit/iconsumepipeconnector), [IRequestPipeConnector](../masstransit/irequestpipeconnector), [IConsumeMessageObserverConnector](../masstransit/iconsumemessageobserverconnector), [IConsumeObserverConnector](../masstransit/iconsumeobserverconnector)

## Properties

### **Connected**

Task is completed once a connection has been made to the consume pipe (any type of consumer, response handler, etc.

```csharp
public abstract Task Connected { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
