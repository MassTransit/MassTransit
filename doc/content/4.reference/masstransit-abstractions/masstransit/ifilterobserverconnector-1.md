---

title: IFilterObserverConnector<TContext>

---

# IFilterObserverConnector\<TContext\>

Namespace: MassTransit

```csharp
public interface IFilterObserverConnector<TContext>
```

#### Type Parameters

`TContext`<br/>

## Methods

### **ConnectObserver(IFilterObserver\<TContext\>)**

Connect an observer to the filter and/or pipe

```csharp
ConnectHandle ConnectObserver(IFilterObserver<TContext> observer)
```

#### Parameters

`observer` [IFilterObserver\<TContext\>](../masstransit/ifilterobserver-1)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>
