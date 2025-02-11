---

title: MessageHandlerMethod<TMessage, T1>

---

# MessageHandlerMethod\<TMessage, T1\>

Namespace: MassTransit.DependencyInjection

```csharp
public class MessageHandlerMethod<TMessage, T1>
```

#### Type Parameters

`TMessage`<br/>

`T1`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageHandlerMethod\<TMessage, T1\>](../masstransit-dependencyinjection/messagehandlermethod-2)

## Properties

### **Handler**

```csharp
public Func<ConsumeContext<TMessage>, T1, Task> Handler { get; }
```

#### Property Value

[Func\<ConsumeContext\<TMessage\>, T1, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

## Constructors

### **MessageHandlerMethod(Func\<ConsumeContext\<TMessage\>, T1, Task\>)**

```csharp
public MessageHandlerMethod(Func<ConsumeContext<TMessage>, T1, Task> handler)
```

#### Parameters

`handler` [Func\<ConsumeContext\<TMessage\>, T1, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

### **MessageHandlerMethod(Func\<TMessage, T1, Task\>)**

```csharp
public MessageHandlerMethod(Func<TMessage, T1, Task> handler)
```

#### Parameters

`handler` [Func\<TMessage, T1, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>
