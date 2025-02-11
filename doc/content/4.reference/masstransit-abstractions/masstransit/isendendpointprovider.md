---

title: ISendEndpointProvider

---

# ISendEndpointProvider

Namespace: MassTransit

The Send Endpoint Provider is used to retrieve endpoints using addresses. The interface is
 available both at the bus and within the context of most message receive handlers, including
 the consume context, saga context, consumer context, etc. The most local provider should be
 used to ensure message continuity is maintained.

```csharp
public interface ISendEndpointProvider : ISendObserverConnector
```

Implements [ISendObserverConnector](../masstransit/isendobserverconnector)

## Methods

### **GetSendEndpoint(Uri)**

Return the send endpoint for the specified address

```csharp
Task<ISendEndpoint> GetSendEndpoint(Uri address)
```

#### Parameters

`address` Uri<br/>
The endpoint address

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The send endpoint
