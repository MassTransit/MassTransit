---

title: RequestHandlerConsumer<TMessage, TResponse>

---

# RequestHandlerConsumer\<TMessage, TResponse\>

Namespace: MassTransit.DependencyInjection

```csharp
public class RequestHandlerConsumer<TMessage, TResponse> : IConsumer<TMessage>, IConsumer
```

#### Type Parameters

`TMessage`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestHandlerConsumer\<TMessage, TResponse\>](../masstransit-dependencyinjection/requesthandlerconsumer-2)<br/>
Implements [IConsumer\<TMessage\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer)

## Constructors

### **RequestHandlerConsumer(RequestHandlerMethod\<TMessage, TResponse\>)**

```csharp
public RequestHandlerConsumer(RequestHandlerMethod<TMessage, TResponse> method)
```

#### Parameters

`method` [RequestHandlerMethod\<TMessage, TResponse\>](../masstransit-dependencyinjection/requesthandlermethod-2)<br/>

## Methods

### **Consume(ConsumeContext\<TMessage\>)**

```csharp
public Task Consume(ConsumeContext<TMessage> context)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
