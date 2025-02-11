---

title: IMessageReceiver<T>

---

# IMessageReceiver\<T\>

Namespace: MassTransit.Transports.Fabric

Receives messages from a queue

```csharp
public interface IMessageReceiver<T> : IProbeSite
```

#### Type Parameters

`T`<br/>

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **Deliver(T, CancellationToken)**

```csharp
Task Deliver(T message, CancellationToken cancellationToken)
```

#### Parameters

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
