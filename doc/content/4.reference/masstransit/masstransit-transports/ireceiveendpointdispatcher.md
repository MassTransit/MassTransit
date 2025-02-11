---

title: IReceiveEndpointDispatcher

---

# IReceiveEndpointDispatcher

Namespace: MassTransit.Transports

```csharp
public interface IReceiveEndpointDispatcher : IConsumeObserverConnector, IConsumeMessageObserverConnector, IDispatchMetrics, IReceiveObserverConnector, IPublishObserverConnector, ISendObserverConnector, IProbeSite
```

Implements [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IDispatchMetrics](../masstransit-transports/idispatchmetrics), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **InputAddress**

```csharp
public abstract Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

## Methods

### **Dispatch(Byte[], IReadOnlyDictionary\<String, Object\>, CancellationToken, Object[])**

Handles the message based upon the endpoint configuration

```csharp
Task Dispatch(Byte[] body, IReadOnlyDictionary<string, object> headers, CancellationToken cancellationToken, Object[] payloads)
```

#### Parameters

`body` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>
The message body

`headers` [IReadOnlyDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br/>
The message headers

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`payloads` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
One or more payloads to add to the receive context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
