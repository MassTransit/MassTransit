---

title: MessageHandlerMethod<TMessage, T1, T2>

---

# MessageHandlerMethod\<TMessage, T1, T2\>

Namespace: MassTransit.DependencyInjection

```csharp
public class MessageHandlerMethod<TMessage, T1, T2>
```

#### Type Parameters

`TMessage`<br/>

`T1`<br/>

`T2`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageHandlerMethod\<TMessage, T1, T2\>](../masstransit-dependencyinjection/messagehandlermethod-3)

## Properties

### **Handler**

```csharp
public Func<ConsumeContext<TMessage>, T1, T2, Task> Handler { get; }
```

#### Property Value

[Func\<ConsumeContext\<TMessage\>, T1, T2, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

## Constructors

### **MessageHandlerMethod(Func\<ConsumeContext\<TMessage\>, T1, T2, Task\>)**

```csharp
public MessageHandlerMethod(Func<ConsumeContext<TMessage>, T1, T2, Task> handler)
```

#### Parameters

`handler` [Func\<ConsumeContext\<TMessage\>, T1, T2, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

### **MessageHandlerMethod(Func\<TMessage, T1, T2, Task\>)**

```csharp
public MessageHandlerMethod(Func<TMessage, T1, T2, Task> handler)
```

#### Parameters

`handler` [Func\<TMessage, T1, T2, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>
