---

title: MessageHandlerMethod<TMessage>

---

# MessageHandlerMethod\<TMessage\>

Namespace: MassTransit.DependencyInjection

```csharp
public class MessageHandlerMethod<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageHandlerMethod\<TMessage\>](../masstransit-dependencyinjection/messagehandlermethod-1)

## Properties

### **Handler**

```csharp
public Func<ConsumeContext<TMessage>, Task> Handler { get; }
```

#### Property Value

[Func\<ConsumeContext\<TMessage\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Constructors

### **MessageHandlerMethod(Func\<ConsumeContext\<TMessage\>, Task\>)**

```csharp
public MessageHandlerMethod(Func<ConsumeContext<TMessage>, Task> handler)
```

#### Parameters

`handler` [Func\<ConsumeContext\<TMessage\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **MessageHandlerMethod(Func\<TMessage, Task\>)**

```csharp
public MessageHandlerMethod(Func<TMessage, Task> handler)
```

#### Parameters

`handler` [Func\<TMessage, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
