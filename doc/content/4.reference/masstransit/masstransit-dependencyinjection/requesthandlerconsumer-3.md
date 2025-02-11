---

title: RequestHandlerConsumer<TMessage, T1, TResponse>

---

# RequestHandlerConsumer\<TMessage, T1, TResponse\>

Namespace: MassTransit.DependencyInjection

```csharp
public class RequestHandlerConsumer<TMessage, T1, TResponse> : IConsumer<TMessage>, IConsumer
```

#### Type Parameters

`TMessage`<br/>

`T1`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestHandlerConsumer\<TMessage, T1, TResponse\>](../masstransit-dependencyinjection/requesthandlerconsumer-3)<br/>
Implements [IConsumer\<TMessage\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer)

## Constructors

### **RequestHandlerConsumer(RequestHandlerMethod\<TMessage, T1, TResponse\>, T1)**

```csharp
public RequestHandlerConsumer(RequestHandlerMethod<TMessage, T1, TResponse> method, T1 arg1)
```

#### Parameters

`method` [RequestHandlerMethod\<TMessage, T1, TResponse\>](../masstransit-dependencyinjection/requesthandlermethod-3)<br/>

`arg1` T1<br/>

## Methods

### **Consume(ConsumeContext\<TMessage\>)**

```csharp
public Task Consume(ConsumeContext<TMessage> context)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
