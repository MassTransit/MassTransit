---

title: RequestHandlerMethod<TMessage, T1, TResponse>

---

# RequestHandlerMethod\<TMessage, T1, TResponse\>

Namespace: MassTransit.DependencyInjection

```csharp
public class RequestHandlerMethod<TMessage, T1, TResponse>
```

#### Type Parameters

`TMessage`<br/>

`T1`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestHandlerMethod\<TMessage, T1, TResponse\>](../masstransit-dependencyinjection/requesthandlermethod-3)

## Properties

### **Handler**

```csharp
public Func<ConsumeContext<TMessage>, T1, Task<TResponse>> Handler { get; }
```

#### Property Value

[Func\<ConsumeContext\<TMessage\>, T1, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

## Constructors

### **RequestHandlerMethod(Func\<ConsumeContext\<TMessage\>, T1, Task\<TResponse\>\>)**

```csharp
public RequestHandlerMethod(Func<ConsumeContext<TMessage>, T1, Task<TResponse>> handler)
```

#### Parameters

`handler` [Func\<ConsumeContext\<TMessage\>, T1, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

### **RequestHandlerMethod(Func\<TMessage, T1, Task\<TResponse\>\>)**

```csharp
public RequestHandlerMethod(Func<TMessage, T1, Task<TResponse>> handler)
```

#### Parameters

`handler` [Func\<TMessage, T1, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>
