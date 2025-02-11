---

title: IReceiveObserverConnector

---

# IReceiveObserverConnector

Namespace: MassTransit

```csharp
public interface IReceiveObserverConnector
```

## Methods

### **ConnectReceiveObserver(IReceiveObserver)**

Connect an observer to the receiving endpoint

```csharp
ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
```

#### Parameters

`observer` [IReceiveObserver](../masstransit/ireceiveobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>
