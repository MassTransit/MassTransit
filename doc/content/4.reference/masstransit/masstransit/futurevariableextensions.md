---

title: FutureVariableExtensions

---

# FutureVariableExtensions

Namespace: MassTransit

```csharp
public static class FutureVariableExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureVariableExtensions](../masstransit/futurevariableextensions)

## Methods

### **SetVariable\<T, TValue\>(BehaviorContext\<FutureState, T\>, String, AsyncEventMessageFactory\<FutureState, T, TValue\>)**

```csharp
public static Task<TValue> SetVariable<T, TValue>(BehaviorContext<FutureState, T> context, string key, AsyncEventMessageFactory<FutureState, T, TValue> factory)
```

#### Type Parameters

`T`<br/>

`TValue`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`factory` [AsyncEventMessageFactory\<FutureState, T, TValue\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SetVariable\<TValue\>(BehaviorContext\<FutureState\>, String, AsyncEventMessageFactory\<FutureState, TValue\>)**

```csharp
public static Task<TValue> SetVariable<TValue>(BehaviorContext<FutureState> context, string key, AsyncEventMessageFactory<FutureState, TValue> factory)
```

#### Type Parameters

`TValue`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`factory` [AsyncEventMessageFactory\<FutureState, TValue\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SetVariable\<T, TValue\>(BehaviorContext\<FutureState, T\>, String, EventMessageFactory\<FutureState, T, TValue\>)**

```csharp
public static TValue SetVariable<T, TValue>(BehaviorContext<FutureState, T> context, string key, EventMessageFactory<FutureState, T, TValue> factory)
```

#### Type Parameters

`T`<br/>

`TValue`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`factory` [EventMessageFactory\<FutureState, T, TValue\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

#### Returns

TValue<br/>

### **SetVariable\<TValue\>(BehaviorContext\<FutureState\>, String, EventMessageFactory\<FutureState, TValue\>)**

```csharp
public static TValue SetVariable<TValue>(BehaviorContext<FutureState> context, string key, EventMessageFactory<FutureState, TValue> factory)
```

#### Type Parameters

`TValue`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`factory` [EventMessageFactory\<FutureState, TValue\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

#### Returns

TValue<br/>

### **SetVariable\<TData, TValue\>(EventActivityBinder\<FutureState, TData\>, String, EventMessageFactory\<FutureState, TData, TValue\>)**

```csharp
public static EventActivityBinder<FutureState, TData> SetVariable<TData, TValue>(EventActivityBinder<FutureState, TData> binder, string key, EventMessageFactory<FutureState, TData, TValue> valueFactory)
```

#### Type Parameters

`TData`<br/>

`TValue`<br/>

#### Parameters

`binder` [EventActivityBinder\<FutureState, TData\>](../masstransit/eventactivitybinder-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`valueFactory` [EventMessageFactory\<FutureState, TData, TValue\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

#### Returns

[EventActivityBinder\<FutureState, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **SetVariable\<TValue\>(EventActivityBinder\<FutureState\>, String, EventMessageFactory\<FutureState, TValue\>)**

```csharp
public static EventActivityBinder<FutureState> SetVariable<TValue>(EventActivityBinder<FutureState> binder, string key, EventMessageFactory<FutureState, TValue> valueFactory)
```

#### Type Parameters

`TValue`<br/>

#### Parameters

`binder` [EventActivityBinder\<FutureState\>](../masstransit/eventactivitybinder-1)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`valueFactory` [EventMessageFactory\<FutureState, TValue\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

#### Returns

[EventActivityBinder\<FutureState\>](../masstransit/eventactivitybinder-1)<br/>

### **SetVariable\<TData, TValue\>(EventActivityBinder\<FutureState, TData\>, String, AsyncEventMessageFactory\<FutureState, TData, TValue\>)**

```csharp
public static EventActivityBinder<FutureState, TData> SetVariable<TData, TValue>(EventActivityBinder<FutureState, TData> binder, string key, AsyncEventMessageFactory<FutureState, TData, TValue> valueFactory)
```

#### Type Parameters

`TData`<br/>

`TValue`<br/>

#### Parameters

`binder` [EventActivityBinder\<FutureState, TData\>](../masstransit/eventactivitybinder-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`valueFactory` [AsyncEventMessageFactory\<FutureState, TData, TValue\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

#### Returns

[EventActivityBinder\<FutureState, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **SetVariable\<TValue\>(EventActivityBinder\<FutureState\>, String, AsyncEventMessageFactory\<FutureState, TValue\>)**

```csharp
public static EventActivityBinder<FutureState> SetVariable<TValue>(EventActivityBinder<FutureState> binder, string key, AsyncEventMessageFactory<FutureState, TValue> valueFactory)
```

#### Type Parameters

`TValue`<br/>

#### Parameters

`binder` [EventActivityBinder\<FutureState\>](../masstransit/eventactivitybinder-1)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`valueFactory` [AsyncEventMessageFactory\<FutureState, TValue\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

#### Returns

[EventActivityBinder\<FutureState\>](../masstransit/eventactivitybinder-1)<br/>

### **SetVariable\<TValue\>(BehaviorContext\<FutureState\>, String, TValue)**

```csharp
public static void SetVariable<TValue>(BehaviorContext<FutureState> context, string key, TValue value)
```

#### Type Parameters

`TValue`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` TValue<br/>

### **TryGetVariable\<T\>(BehaviorContext\<FutureState\>, String, T)**

```csharp
public static bool TryGetVariable<T>(BehaviorContext<FutureState> context, string key, out T result)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<FutureState\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
