---

title: RequestHandlerMethod<TMessage, T1, T2, TResponse>

---

# RequestHandlerMethod\<TMessage, T1, T2, TResponse\>

Namespace: MassTransit.DependencyInjection

```csharp
public class RequestHandlerMethod<TMessage, T1, T2, TResponse>
```

#### Type Parameters

`TMessage`<br/>

`T1`<br/>

`T2`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestHandlerMethod\<TMessage, T1, T2, TResponse\>](../masstransit-dependencyinjection/requesthandlermethod-4)

## Properties

### **Handler**

```csharp
public Func<ConsumeContext<TMessage>, T1, T2, Task<TResponse>> Handler { get; }
```

#### Property Value

[Func\<ConsumeContext\<TMessage\>, T1, T2, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

## Constructors

### **RequestHandlerMethod(Func\<ConsumeContext\<TMessage\>, T1, T2, Task\<TResponse\>\>)**

```csharp
public RequestHandlerMethod(Func<ConsumeContext<TMessage>, T1, T2, Task<TResponse>> handler)
```

#### Parameters

`handler` [Func\<ConsumeContext\<TMessage\>, T1, T2, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

### **RequestHandlerMethod(Func\<TMessage, T1, T2, Task\<TResponse\>\>)**

```csharp
public RequestHandlerMethod(Func<TMessage, T1, T2, Task<TResponse>> handler)
```

#### Parameters

`handler` [Func\<TMessage, T1, T2, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>
