---

title: IFilterObserverConnector

---

# IFilterObserverConnector

Namespace: MassTransit

```csharp
public interface IFilterObserverConnector
```

## Methods

### **ConnectObserver\<T\>(IFilterObserver\<T\>)**

Connect an observer to the filter and/or pipe

```csharp
ConnectHandle ConnectObserver<T>(IFilterObserver<T> observer)
```

#### Type Parameters

`T`<br/>

#### Parameters

`observer` [IFilterObserver\<T\>](../masstransit/ifilterobserver-1)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **ConnectObserver(IFilterObserver)**

Connect an observer to the filter and/or pipe

```csharp
ConnectHandle ConnectObserver(IFilterObserver observer)
```

#### Parameters

`observer` [IFilterObserver](../masstransit/ifilterobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>
