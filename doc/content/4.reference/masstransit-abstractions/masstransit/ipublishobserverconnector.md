---

title: IPublishObserverConnector

---

# IPublishObserverConnector

Namespace: MassTransit

Connect an observer that is notified when a message is sent to an endpoint

```csharp
public interface IPublishObserverConnector
```

## Methods

### **ConnectPublishObserver(IPublishObserver)**

```csharp
ConnectHandle ConnectPublishObserver(IPublishObserver observer)
```

#### Parameters

`observer` [IPublishObserver](../masstransit/ipublishobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>
