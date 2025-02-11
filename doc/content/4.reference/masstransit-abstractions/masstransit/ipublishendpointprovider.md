---

title: IPublishEndpointProvider

---

# IPublishEndpointProvider

Namespace: MassTransit

```csharp
public interface IPublishEndpointProvider : IPublishObserverConnector
```

Implements [IPublishObserverConnector](../masstransit/ipublishobserverconnector)

## Methods

### **GetPublishSendEndpoint\<T\>()**

Return the SendEndpoint used for publishing the specified message

```csharp
Task<ISendEndpoint> GetPublishSendEndpoint<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
