---

title: DelaySendPipe<T>

---

# DelaySendPipe\<T\>

Namespace: MassTransit.Middleware

```csharp
public class DelaySendPipe<T> : IPipe<SendContext<T>>, IProbeSite
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelaySendPipe\<T\>](../masstransit-middleware/delaysendpipe-1)<br/>
Implements [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **DelaySendPipe(IPipe\<SendContext\<T\>\>, TimeSpan)**

```csharp
public DelaySendPipe(IPipe<SendContext<T>> pipe, TimeSpan delay)
```

#### Parameters

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Methods

### **Send(SendContext\<T\>)**

```csharp
public Task Send(SendContext<T> context)
```

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
