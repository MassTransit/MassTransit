---

title: IRetryObserverConnector

---

# IRetryObserverConnector

Namespace: MassTransit

```csharp
public interface IRetryObserverConnector
```

## Methods

### **ConnectRetryObserver(IRetryObserver)**

Connect an observer to the filter and/or pipe

```csharp
ConnectHandle ConnectRetryObserver(IRetryObserver observer)
```

#### Parameters

`observer` [IRetryObserver](../masstransit/iretryobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>
