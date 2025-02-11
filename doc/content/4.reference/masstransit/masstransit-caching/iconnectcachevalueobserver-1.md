---

title: IConnectCacheValueObserver<TValue>

---

# IConnectCacheValueObserver\<TValue\>

Namespace: MassTransit.Caching

```csharp
public interface IConnectCacheValueObserver<TValue>
```

#### Type Parameters

`TValue`<br/>

## Methods

### **Connect(ICacheValueObserver\<TValue\>)**

```csharp
ConnectHandle Connect(ICacheValueObserver<TValue> observer)
```

#### Parameters

`observer` [ICacheValueObserver\<TValue\>](../masstransit-caching/icachevalueobserver-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
