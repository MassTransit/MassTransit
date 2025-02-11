---

title: TransactionalBusPublishEndpointProvider

---

# TransactionalBusPublishEndpointProvider

Namespace: MassTransit.Transactions

```csharp
public class TransactionalBusPublishEndpointProvider : IPublishEndpointProvider, IPublishObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransactionalBusPublishEndpointProvider](../masstransit-transactions/transactionalbuspublishendpointprovider)<br/>
Implements [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector)

## Constructors

### **TransactionalBusPublishEndpointProvider(BaseTransactionalBus, IPublishEndpointProvider)**

```csharp
public TransactionalBusPublishEndpointProvider(BaseTransactionalBus bus, IPublishEndpointProvider publishEndpointProvider)
```

#### Parameters

`bus` [BaseTransactionalBus](../masstransit-transactions/basetransactionalbus)<br/>

`publishEndpointProvider` [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider)<br/>

## Methods

### **ConnectPublishObserver(IPublishObserver)**

```csharp
public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
```

#### Parameters

`observer` [IPublishObserver](../../masstransit-abstractions/masstransit/ipublishobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **GetPublishSendEndpoint\<T\>()**

```csharp
public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
