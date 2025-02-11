---

title: PublishSendPipeAdapter<T>

---

# PublishSendPipeAdapter\<T\>

Namespace: MassTransit.Transports

```csharp
public class PublishSendPipeAdapter<T> : IPipe<SendContext<T>>, IProbeSite
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishSendPipeAdapter\<T\>](../masstransit-transports/publishsendpipeadapter-1)<br/>
Implements [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1), [IProbeSite](../masstransit/iprobesite)

## Constructors

### **PublishSendPipeAdapter(IPipe\<PublishContext\<T\>\>)**

```csharp
public PublishSendPipeAdapter(IPipe<PublishContext<T>> pipe)
```

#### Parameters

`pipe` [IPipe\<PublishContext\<T\>\>](../masstransit/ipipe-1)<br/>

## Methods

### **Send(SendContext\<T\>)**

```csharp
public Task Send(SendContext<T> context)
```

#### Parameters

`context` [SendContext\<T\>](../masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
