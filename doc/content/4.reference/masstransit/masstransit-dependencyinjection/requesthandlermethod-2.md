---

title: RequestHandlerMethod<TMessage, TResponse>

---

# RequestHandlerMethod\<TMessage, TResponse\>

Namespace: MassTransit.DependencyInjection

```csharp
public class RequestHandlerMethod<TMessage, TResponse>
```

#### Type Parameters

`TMessage`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestHandlerMethod\<TMessage, TResponse\>](../masstransit-dependencyinjection/requesthandlermethod-2)

## Properties

### **Handler**

```csharp
public Func<ConsumeContext<TMessage>, Task<TResponse>> Handler { get; }
```

#### Property Value

[Func\<ConsumeContext\<TMessage\>, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Constructors

### **RequestHandlerMethod(Func\<ConsumeContext\<TMessage\>, Task\<TResponse\>\>)**

```csharp
public RequestHandlerMethod(Func<ConsumeContext<TMessage>, Task<TResponse>> handler)
```

#### Parameters

`handler` [Func\<ConsumeContext\<TMessage\>, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **RequestHandlerMethod(Func\<TMessage, Task\<TResponse\>\>)**

```csharp
public RequestHandlerMethod(Func<TMessage, Task<TResponse>> handler)
```

#### Parameters

`handler` [Func\<TMessage, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
