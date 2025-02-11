---

title: FutureSubscriptionExtensions

---

# FutureSubscriptionExtensions

Namespace: MassTransit.Futures

```csharp
public static class FutureSubscriptionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureSubscriptionExtensions](../masstransit-futures/futuresubscriptionextensions)

## Methods

### **SendMessageToSubscriptions\<T\>(BehaviorContext\<FutureState\>, ContextMessageFactory\<BehaviorContext\<FutureState\>, T\>, IEnumerable\<FutureSubscription\>)**

```csharp
public static Task<T> SendMessageToSubscriptions<T>(BehaviorContext<FutureState> context, ContextMessageFactory<BehaviorContext<FutureState>, T> factory, IEnumerable<FutureSubscription> subscriptions)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`factory` [ContextMessageFactory\<BehaviorContext\<FutureState\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

`subscriptions` [IEnumerable\<FutureSubscription\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SendMessageToSubscriptions\<TInput, T\>(BehaviorContext\<FutureState, TInput\>, ContextMessageFactory\<BehaviorContext\<FutureState, TInput\>, T\>, IEnumerable\<FutureSubscription\>)**

```csharp
public static Task<T> SendMessageToSubscriptions<TInput, T>(BehaviorContext<FutureState, TInput> context, ContextMessageFactory<BehaviorContext<FutureState, TInput>, T> factory, IEnumerable<FutureSubscription> subscriptions)
```

#### Type Parameters

`TInput`<br/>

`T`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState, TInput\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`factory` [ContextMessageFactory\<BehaviorContext\<FutureState, TInput\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

`subscriptions` [IEnumerable\<FutureSubscription\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
