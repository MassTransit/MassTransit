---

title: ISendObserverConnector

---

# ISendObserverConnector

Namespace: MassTransit

Connect an observer that is notified when a message is sent to an endpoint

```csharp
public interface ISendObserverConnector
```

## Methods

### **ConnectSendObserver(ISendObserver)**

```csharp
ConnectHandle ConnectSendObserver(ISendObserver observer)
```

#### Parameters

`observer` [ISendObserver](../masstransit/isendobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>
