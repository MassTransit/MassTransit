---

title: MessageRetryPolicyExtensions

---

# MessageRetryPolicyExtensions

Namespace: MassTransit.RetryPolicies

```csharp
public static class MessageRetryPolicyExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageRetryPolicyExtensions](../masstransit-retrypolicies/messageretrypolicyextensions)

## Methods

### **Retry\<T\>(IRetryPolicy, ConsumeContext\<T\>, Func\<ConsumeContext\<T\>, Task\>)**

```csharp
public static Task Retry<T>(IRetryPolicy retryPolicy, ConsumeContext<T> context, Func<ConsumeContext<T>, Task> retryMethod)
```

#### Type Parameters

`T`<br/>

#### Parameters

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`retryMethod` [Func\<ConsumeContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
