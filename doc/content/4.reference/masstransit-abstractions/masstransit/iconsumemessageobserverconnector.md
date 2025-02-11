---

title: IConsumeMessageObserverConnector

---

# IConsumeMessageObserverConnector

Namespace: MassTransit

Supports connection of a message observer to the pipeline

```csharp
public interface IConsumeMessageObserverConnector
```

## Methods

### **ConnectConsumeMessageObserver\<T\>(IConsumeMessageObserver\<T\>)**

```csharp
ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
```

#### Type Parameters

`T`<br/>

#### Parameters

`observer` [IConsumeMessageObserver\<T\>](../masstransit/iconsumemessageobserver-1)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>
