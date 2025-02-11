---

title: MediatorRequestHandler<TRequest, TResponse>

---

# MediatorRequestHandler\<TRequest, TResponse\>

Namespace: MassTransit.Mediator

A Mediator request handler base class that provides a simplified overridable method with
 a Task&lt;&gt; return type

```csharp
public abstract class MediatorRequestHandler<TRequest, TResponse> : IConsumer<TRequest>, IConsumer
```

#### Type Parameters

`TRequest`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MediatorRequestHandler\<TRequest, TResponse\>](../masstransit-mediator/mediatorrequesthandler-2)<br/>
Implements [IConsumer\<TRequest\>](../masstransit/iconsumer-1), [IConsumer](../masstransit/iconsumer)

## Methods

### **Consume(ConsumeContext\<TRequest\>)**

```csharp
public Task Consume(ConsumeContext<TRequest> context)
```

#### Parameters

`context` [ConsumeContext\<TRequest\>](../masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Handle(TRequest, CancellationToken)**

```csharp
protected abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
```

#### Parameters

`request` TRequest<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<TResponse\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
