---

title: FutureStateExtensions

---

# FutureStateExtensions

Namespace: MassTransit

```csharp
public static class FutureStateExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureStateExtensions](../masstransit/futurestateextensions)

## Methods

### **GetCommand\<T\>(BehaviorContext\<FutureState\>)**

```csharp
public static T GetCommand<T>(BehaviorContext<FutureState> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

#### Returns

T<br/>

### **ToObject\<T\>(BehaviorContext\<FutureState\>, FutureMessage)**

```csharp
public static T ToObject<T>(BehaviorContext<FutureState> context, FutureMessage message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`message` [FutureMessage](../../masstransit-abstractions/masstransit/futuremessage)<br/>

#### Returns

T<br/>

### **CreateFutureMessage\<T\>(BehaviorContext\<FutureState\>, T)**

```csharp
public static FutureMessage CreateFutureMessage<T>(BehaviorContext<FutureState> context, T message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`message` T<br/>

#### Returns

[FutureMessage](../../masstransit-abstractions/masstransit/futuremessage)<br/>

### **SelectResults\<T\>(BehaviorContext\<FutureState\>)**

```csharp
public static IEnumerable<T> SelectResults<T>(BehaviorContext<FutureState> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **AddSubscription(BehaviorContext\<FutureState\>)**

```csharp
public static void AddSubscription(BehaviorContext<FutureState> context)
```

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

### **SetResult\<T, TResult\>(BehaviorContext\<FutureState, T\>, Guid, AsyncEventMessageFactory\<FutureState, T, TResult\>)**

```csharp
public static Task<TResult> SetResult<T, TResult>(BehaviorContext<FutureState, T> context, Guid id, AsyncEventMessageFactory<FutureState, T, TResult> factory)
```

#### Type Parameters

`T`<br/>

`TResult`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`factory` [AsyncEventMessageFactory\<FutureState, T, TResult\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

#### Returns

[Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SetResult\<TResult\>(BehaviorContext\<FutureState\>, Guid, AsyncEventMessageFactory\<FutureState, TResult\>)**

```csharp
public static Task<TResult> SetResult<TResult>(BehaviorContext<FutureState> context, Guid id, AsyncEventMessageFactory<FutureState, TResult> factory)
```

#### Type Parameters

`TResult`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`factory` [AsyncEventMessageFactory\<FutureState, TResult\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

#### Returns

[Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SetResult\<T, TResult\>(BehaviorContext\<FutureState, T\>, Guid, EventMessageFactory\<FutureState, T, TResult\>)**

```csharp
public static void SetResult<T, TResult>(BehaviorContext<FutureState, T> context, Guid id, EventMessageFactory<FutureState, T, TResult> factory)
```

#### Type Parameters

`T`<br/>

`TResult`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`factory` [EventMessageFactory\<FutureState, T, TResult\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

### **SetResult\<TResult\>(BehaviorContext\<FutureState\>, Guid, EventMessageFactory\<FutureState, TResult\>)**

```csharp
public static TResult SetResult<TResult>(BehaviorContext<FutureState> context, Guid id, EventMessageFactory<FutureState, TResult> factory)
```

#### Type Parameters

`TResult`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`factory` [EventMessageFactory\<FutureState, TResult\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

#### Returns

TResult<br/>

### **SetResult\<TResult\>(BehaviorContext\<FutureState\>, Guid, TResult)**

```csharp
public static void SetResult<TResult>(BehaviorContext<FutureState> context, Guid id, TResult result)
```

#### Type Parameters

`TResult`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`result` TResult<br/>

### **SetCompleted(BehaviorContext\<FutureState\>, Guid)**

```csharp
public static void SetCompleted(BehaviorContext<FutureState> context, Guid id)
```

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **SetFaulted(BehaviorContext\<FutureState\>, Guid, Nullable\<DateTime\>)**

```csharp
public static void SetFaulted(BehaviorContext<FutureState> context, Guid id, Nullable<DateTime> timestamp)
```

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SetFault\<TFault\>(BehaviorContext\<FutureState\>, Guid, TFault, Nullable\<DateTime\>)**

```csharp
public static void SetFault<TFault>(BehaviorContext<FutureState> context, Guid id, TFault fault, Nullable<DateTime> timestamp)
```

#### Type Parameters

`TFault`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`fault` TFault<br/>

`timestamp` [Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SetFault\<T, TFault\>(BehaviorContext\<FutureState, T\>, Guid, EventMessageFactory\<FutureState, T, TFault\>)**

```csharp
public static void SetFault<T, TFault>(BehaviorContext<FutureState, T> context, Guid id, EventMessageFactory<FutureState, T, TFault> factory)
```

#### Type Parameters

`T`<br/>

`TFault`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`factory` [EventMessageFactory\<FutureState, T, TFault\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

### **SetFault\<T, TFault\>(FutureState, BehaviorContext\<FutureState, T\>, Guid, AsyncEventMessageFactory\<FutureState, T, TFault\>)**

```csharp
public static Task<TFault> SetFault<T, TFault>(FutureState future, BehaviorContext<FutureState, T> context, Guid id, AsyncEventMessageFactory<FutureState, T, TFault> factory)
```

#### Type Parameters

`T`<br/>

`TFault`<br/>

#### Parameters

`future` [FutureState](../../masstransit-abstractions/masstransit/futurestate)<br/>

`context` [BehaviorContext\<FutureState, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`factory` [AsyncEventMessageFactory\<FutureState, T, TFault\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

#### Returns

[Task\<TFault\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **TryGetResult\<T\>(BehaviorContext\<FutureState\>, Guid, T)**

```csharp
public static bool TryGetResult<T>(BehaviorContext<FutureState> context, Guid id, out T result)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`result` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetFault\<T\>(BehaviorContext\<FutureState\>, Guid, T)**

```csharp
public static bool TryGetFault<T>(BehaviorContext<FutureState> context, Guid id, out T fault)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`fault` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
