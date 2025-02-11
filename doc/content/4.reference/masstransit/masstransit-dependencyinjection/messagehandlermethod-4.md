---

title: MessageHandlerMethod<TMessage, T1, T2, T3>

---

# MessageHandlerMethod\<TMessage, T1, T2, T3\>

Namespace: MassTransit.DependencyInjection

```csharp
public class MessageHandlerMethod<TMessage, T1, T2, T3>
```

#### Type Parameters

`TMessage`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageHandlerMethod\<TMessage, T1, T2, T3\>](../masstransit-dependencyinjection/messagehandlermethod-4)

## Properties

### **Handler**

```csharp
public Func<ConsumeContext<TMessage>, T1, T2, T3, Task> Handler { get; }
```

#### Property Value

[Func\<ConsumeContext\<TMessage\>, T1, T2, T3, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>

## Constructors

### **MessageHandlerMethod(Func\<ConsumeContext\<TMessage\>, T1, T2, T3, Task\>)**

```csharp
public MessageHandlerMethod(Func<ConsumeContext<TMessage>, T1, T2, T3, Task> handler)
```

#### Parameters

`handler` [Func\<ConsumeContext\<TMessage\>, T1, T2, T3, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>

### **MessageHandlerMethod(Func\<TMessage, T1, T2, T3, Task\>)**

```csharp
public MessageHandlerMethod(Func<TMessage, T1, T2, T3, Task> handler)
```

#### Parameters

`handler` [Func\<TMessage, T1, T2, T3, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>
