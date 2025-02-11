---

title: RequestHandlerConsumer<TMessage, T1, T2, TResponse>

---

# RequestHandlerConsumer\<TMessage, T1, T2, TResponse\>

Namespace: MassTransit.DependencyInjection

```csharp
public class RequestHandlerConsumer<TMessage, T1, T2, TResponse> : IConsumer<TMessage>, IConsumer
```

#### Type Parameters

`TMessage`<br/>

`T1`<br/>

`T2`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestHandlerConsumer\<TMessage, T1, T2, TResponse\>](../masstransit-dependencyinjection/requesthandlerconsumer-4)<br/>
Implements [IConsumer\<TMessage\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer)

## Constructors

### **RequestHandlerConsumer(RequestHandlerMethod\<TMessage, T1, T2, TResponse\>, T1, T2)**

```csharp
public RequestHandlerConsumer(RequestHandlerMethod<TMessage, T1, T2, TResponse> method, T1 arg1, T2 arg2)
```

#### Parameters

`method` [RequestHandlerMethod\<TMessage, T1, T2, TResponse\>](../masstransit-dependencyinjection/requesthandlermethod-4)<br/>

`arg1` T1<br/>

`arg2` T2<br/>

## Methods

### **Consume(ConsumeContext\<TMessage\>)**

```csharp
public Task Consume(ConsumeContext<TMessage> context)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
