---

title: IConsumeMessageObserverConnector<T>

---

# IConsumeMessageObserverConnector\<T\>

Namespace: MassTransit

Supports connection of a message observer to the pipeline

```csharp
public interface IConsumeMessageObserverConnector<T>
```

#### Type Parameters

`T`<br/>

## Methods

### **ConnectConsumeMessageObserver(IConsumeMessageObserver\<T\>)**

```csharp
ConnectHandle ConnectConsumeMessageObserver(IConsumeMessageObserver<T> observer)
```

#### Parameters

`observer` [IConsumeMessageObserver\<T\>](../masstransit/iconsumemessageobserver-1)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>
