---

title: IReceivePipeDispatcher

---

# IReceivePipeDispatcher

Namespace: MassTransit.Transports

Dispatches a prepared  to a .

```csharp
public interface IReceivePipeDispatcher : IConsumePipeConnector, IConsumeObserverConnector, IConsumeMessageObserverConnector, IRequestPipeConnector, IDispatchMetrics, IReceiveObserverConnector, IProbeSite
```

Implements [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector), [IDispatchMetrics](../masstransit-transports/idispatchmetrics), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **Dispatch(ReceiveContext, ReceiveLockContext)**

```csharp
Task Dispatch(ReceiveContext context, ReceiveLockContext receiveLock)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

`receiveLock` [ReceiveLockContext](../masstransit-transports/receivelockcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
