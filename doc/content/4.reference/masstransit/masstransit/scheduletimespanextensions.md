---

title: ScheduleTimeSpanExtensions

---

# ScheduleTimeSpanExtensions

Namespace: MassTransit

```csharp
public static class ScheduleTimeSpanExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduleTimeSpanExtensions](../masstransit/scheduletimespanextensions)

## Methods

### **Schedule\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Schedule\<TSaga, TMessage\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(EventActivityBinder<TSaga> source, Schedule<TSaga, TMessage> schedule, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Schedule\<TSaga, TMessage\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(EventActivityBinder<TSaga> source, Schedule<TSaga, TMessage> schedule, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Schedule\<TSaga, TMessage\>, TMessage, ScheduleDelayProvider\<TSaga\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(EventActivityBinder<TSaga> source, Schedule<TSaga, TMessage> schedule, TMessage message, ScheduleDelayProvider<TSaga> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` TMessage<br/>

`delayProvider` [ScheduleDelayProvider\<TSaga\>](../../masstransit-abstractions/masstransit/scheduledelayprovider-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Schedule\<TSaga, TMessage\>, Task\<TMessage\>, ScheduleDelayProvider\<TSaga\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(EventActivityBinder<TSaga> source, Schedule<TSaga, TMessage> schedule, Task<TMessage> message, ScheduleDelayProvider<TSaga> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`delayProvider` [ScheduleDelayProvider\<TSaga\>](../../masstransit-abstractions/masstransit/scheduledelayprovider-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Schedule\<TSaga, TMessage\>, EventMessageFactory\<TSaga, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(EventActivityBinder<TSaga> source, Schedule<TSaga, TMessage> schedule, EventMessageFactory<TSaga, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [EventMessageFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Schedule\<TSaga, TMessage\>, AsyncEventMessageFactory\<TSaga, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(EventActivityBinder<TSaga> source, Schedule<TSaga, TMessage> schedule, AsyncEventMessageFactory<TSaga, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [AsyncEventMessageFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Schedule\<TSaga, TMessage\>, Func\<BehaviorContext\<TSaga\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(EventActivityBinder<TSaga> source, Schedule<TSaga, TMessage> schedule, Func<BehaviorContext<TSaga>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [Func\<BehaviorContext\<TSaga\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Schedule\<TSaga, TMessage\>, EventMessageFactory\<TSaga, TMessage\>, ScheduleDelayProvider\<TSaga\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(EventActivityBinder<TSaga> source, Schedule<TSaga, TMessage> schedule, EventMessageFactory<TSaga, TMessage> messageFactory, ScheduleDelayProvider<TSaga> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [EventMessageFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

`delayProvider` [ScheduleDelayProvider\<TSaga\>](../../masstransit-abstractions/masstransit/scheduledelayprovider-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Schedule\<TSaga, TMessage\>, AsyncEventMessageFactory\<TSaga, TMessage\>, ScheduleDelayProvider\<TSaga\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(EventActivityBinder<TSaga> source, Schedule<TSaga, TMessage> schedule, AsyncEventMessageFactory<TSaga, TMessage> messageFactory, ScheduleDelayProvider<TSaga> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [AsyncEventMessageFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

`delayProvider` [ScheduleDelayProvider\<TSaga\>](../../masstransit-abstractions/masstransit/scheduledelayprovider-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TSaga, TMessage\>(EventActivityBinder\<TSaga\>, Schedule\<TSaga, TMessage\>, Func\<BehaviorContext\<TSaga\>, Task\<SendTuple\<TMessage\>\>\>, ScheduleDelayProvider\<TSaga\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(EventActivityBinder<TSaga> source, Schedule<TSaga, TMessage> schedule, Func<BehaviorContext<TSaga>, Task<SendTuple<TMessage>>> messageFactory, ScheduleDelayProvider<TSaga> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [Func\<BehaviorContext\<TSaga\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`delayProvider` [ScheduleDelayProvider\<TSaga\>](../../masstransit-abstractions/masstransit/scheduledelayprovider-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Schedule\<TSaga, TMessage\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Schedule<TSaga, TMessage> schedule, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Schedule\<TSaga, TMessage\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Schedule<TSaga, TMessage> schedule, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Schedule\<TSaga, TMessage\>, TMessage, ScheduleDelayProvider\<TSaga, TData\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Schedule<TSaga, TMessage> schedule, TMessage message, ScheduleDelayProvider<TSaga, TData> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` TMessage<br/>

`delayProvider` [ScheduleDelayProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/scheduledelayprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Schedule\<TSaga, TMessage\>, Task\<TMessage\>, ScheduleDelayProvider\<TSaga, TData\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Schedule<TSaga, TMessage> schedule, Task<TMessage> message, ScheduleDelayProvider<TSaga, TData> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`delayProvider` [ScheduleDelayProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/scheduledelayprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Schedule\<TSaga, TMessage\>, EventMessageFactory\<TSaga, TData, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Schedule<TSaga, TMessage> schedule, EventMessageFactory<TSaga, TData, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [EventMessageFactory\<TSaga, TData, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Schedule\<TSaga, TMessage\>, AsyncEventMessageFactory\<TSaga, TData, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Schedule<TSaga, TMessage> schedule, AsyncEventMessageFactory<TSaga, TData, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [AsyncEventMessageFactory\<TSaga, TData, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Schedule\<TSaga, TMessage\>, Func\<BehaviorContext\<TSaga, TData\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Schedule<TSaga, TMessage> schedule, Func<BehaviorContext<TSaga, TData>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [Func\<BehaviorContext\<TSaga, TData\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Schedule\<TSaga, TMessage\>, EventMessageFactory\<TSaga, TData, TMessage\>, ScheduleDelayProvider\<TSaga, TData\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Schedule<TSaga, TMessage> schedule, EventMessageFactory<TSaga, TData, TMessage> messageFactory, ScheduleDelayProvider<TSaga, TData> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [EventMessageFactory\<TSaga, TData, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

`delayProvider` [ScheduleDelayProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/scheduledelayprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Schedule\<TSaga, TMessage\>, AsyncEventMessageFactory\<TSaga, TData, TMessage\>, ScheduleDelayProvider\<TSaga, TData\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Schedule<TSaga, TMessage> schedule, AsyncEventMessageFactory<TSaga, TData, TMessage> messageFactory, ScheduleDelayProvider<TSaga, TData> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [AsyncEventMessageFactory\<TSaga, TData, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

`delayProvider` [ScheduleDelayProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/scheduledelayprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TSaga, TData, TMessage\>(EventActivityBinder\<TSaga, TData\>, Schedule\<TSaga, TMessage\>, Func\<BehaviorContext\<TSaga, TData\>, Task\<SendTuple\<TMessage\>\>\>, ScheduleDelayProvider\<TSaga, TData\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(EventActivityBinder<TSaga, TData> source, Schedule<TSaga, TMessage> schedule, Func<BehaviorContext<TSaga, TData>, Task<SendTuple<TMessage>>> messageFactory, ScheduleDelayProvider<TSaga, TData> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [Func\<BehaviorContext\<TSaga, TData\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`delayProvider` [ScheduleDelayProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/scheduledelayprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Schedule\<TSaga, TMessage\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Schedule<TSaga, TMessage> schedule, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Schedule\<TSaga, TMessage\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Schedule<TSaga, TMessage> schedule, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Schedule\<TSaga, TMessage\>, TMessage, ScheduleDelayExceptionProvider\<TSaga, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Schedule<TSaga, TMessage> schedule, TMessage message, ScheduleDelayExceptionProvider<TSaga, TException> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` TMessage<br/>

`delayProvider` [ScheduleDelayExceptionProvider\<TSaga, TException\>](../../masstransit-abstractions/masstransit/scheduledelayexceptionprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Schedule\<TSaga, TMessage\>, Task\<TMessage\>, ScheduleDelayExceptionProvider\<TSaga, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Schedule<TSaga, TMessage> schedule, Task<TMessage> message, ScheduleDelayExceptionProvider<TSaga, TException> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`delayProvider` [ScheduleDelayExceptionProvider\<TSaga, TException\>](../../masstransit-abstractions/masstransit/scheduledelayexceptionprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Schedule\<TSaga, TMessage\>, EventExceptionMessageFactory\<TSaga, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Schedule<TSaga, TMessage> schedule, EventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [EventExceptionMessageFactory\<TSaga, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Schedule\<TSaga, TMessage\>, AsyncEventExceptionMessageFactory\<TSaga, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Schedule<TSaga, TMessage> schedule, AsyncEventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TSaga, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Schedule\<TSaga, TMessage\>, Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Schedule<TSaga, TMessage> schedule, Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Schedule\<TSaga, TMessage\>, EventExceptionMessageFactory\<TSaga, TException, TMessage\>, ScheduleDelayExceptionProvider\<TSaga, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Schedule<TSaga, TMessage> schedule, EventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory, ScheduleDelayExceptionProvider<TSaga, TException> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [EventExceptionMessageFactory\<TSaga, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-3)<br/>

`delayProvider` [ScheduleDelayExceptionProvider\<TSaga, TException\>](../../masstransit-abstractions/masstransit/scheduledelayexceptionprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Schedule\<TSaga, TMessage\>, AsyncEventExceptionMessageFactory\<TSaga, TException, TMessage\>, ScheduleDelayExceptionProvider\<TSaga, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Schedule<TSaga, TMessage> schedule, AsyncEventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory, ScheduleDelayExceptionProvider<TSaga, TException> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TSaga, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-3)<br/>

`delayProvider` [ScheduleDelayExceptionProvider\<TSaga, TException\>](../../masstransit-abstractions/masstransit/scheduledelayexceptionprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TSaga, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TException\>, Schedule\<TSaga, TMessage\>, Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\<SendTuple\<TMessage\>\>\>, ScheduleDelayExceptionProvider\<TSaga, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(ExceptionActivityBinder<TSaga, TException> source, Schedule<TSaga, TMessage> schedule, Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<TMessage>>> messageFactory, ScheduleDelayExceptionProvider<TSaga, TException> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`delayProvider` [ScheduleDelayExceptionProvider\<TSaga, TException\>](../../masstransit-abstractions/masstransit/scheduledelayexceptionprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Schedule\<TSaga, TMessage\>, TMessage, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule, TMessage message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` TMessage<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Schedule\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Schedule\<TSaga, TMessage\>, Task\<TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule, Task<TMessage> message, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Schedule\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Schedule\<TSaga, TMessage\>, TMessage, ScheduleDelayExceptionProvider\<TSaga, TData, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule, TMessage message, ScheduleDelayExceptionProvider<TSaga, TData, TException> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` TMessage<br/>

`delayProvider` [ScheduleDelayExceptionProvider\<TSaga, TData, TException\>](../../masstransit-abstractions/masstransit/scheduledelayexceptionprovider-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Schedule\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Schedule\<TSaga, TMessage\>, Task\<TMessage\>, ScheduleDelayExceptionProvider\<TSaga, TData, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule, Task<TMessage> message, ScheduleDelayExceptionProvider<TSaga, TData, TException> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`delayProvider` [ScheduleDelayExceptionProvider\<TSaga, TData, TException\>](../../masstransit-abstractions/masstransit/scheduledelayexceptionprovider-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Schedule\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Schedule\<TSaga, TMessage\>, EventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule, EventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [EventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-4)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Schedule\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Schedule\<TSaga, TMessage\>, AsyncEventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule, AsyncEventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-4)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Schedule\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Schedule\<TSaga, TMessage\>, Func\<BehaviorExceptionContext\<TSaga, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule, Func<BehaviorExceptionContext<TSaga, TData, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TSaga, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Schedule\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Schedule\<TSaga, TMessage\>, EventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>, ScheduleDelayExceptionProvider\<TSaga, TData, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule, EventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory, ScheduleDelayExceptionProvider<TSaga, TData, TException> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [EventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-4)<br/>

`delayProvider` [ScheduleDelayExceptionProvider\<TSaga, TData, TException\>](../../masstransit-abstractions/masstransit/scheduledelayexceptionprovider-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Schedule\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Schedule\<TSaga, TMessage\>, AsyncEventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>, ScheduleDelayExceptionProvider\<TSaga, TData, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule, AsyncEventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory, ScheduleDelayExceptionProvider<TSaga, TData, TException> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TSaga, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-4)<br/>

`delayProvider` [ScheduleDelayExceptionProvider\<TSaga, TData, TException\>](../../masstransit-abstractions/masstransit/scheduledelayexceptionprovider-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Schedule\<TSaga, TData, TException, TMessage\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Schedule\<TSaga, TMessage\>, Func\<BehaviorExceptionContext\<TSaga, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>, ScheduleDelayExceptionProvider\<TSaga, TData, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule, Func<BehaviorExceptionContext<TSaga, TData, TException>, Task<SendTuple<TMessage>>> messageFactory, ScheduleDelayExceptionProvider<TSaga, TData, TException> delayProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TSaga, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`delayProvider` [ScheduleDelayExceptionProvider\<TSaga, TData, TException\>](../../masstransit-abstractions/masstransit/scheduledelayexceptionprovider-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Unschedule\<TSaga, TData\>(EventActivityBinder\<TSaga, TData\>, Schedule\<TSaga\>)**

Unschedule a message, if the message was scheduled.

```csharp
public static EventActivityBinder<TSaga, TData> Unschedule<TSaga, TData>(EventActivityBinder<TSaga, TData> source, Schedule<TSaga> schedule)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga\>](../../masstransit-abstractions/masstransit/schedule-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Unschedule\<TSaga, TData, TException\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Schedule\<TSaga\>)**

Unschedule a message, if the message was scheduled.

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Unschedule<TSaga, TData, TException>(ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga> schedule)
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TSaga\>](../../masstransit-abstractions/masstransit/schedule-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Unschedule\<TSaga\>(EventActivityBinder\<TSaga\>, Schedule\<TSaga\>)**

Unschedule a message, if the message was scheduled.

```csharp
public static EventActivityBinder<TSaga> Unschedule<TSaga>(EventActivityBinder<TSaga> source, Schedule<TSaga> schedule)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TSaga\>](../../masstransit-abstractions/masstransit/schedule-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Unschedule\<TSaga, TException\>(ExceptionActivityBinder\<TSaga, TException\>, Schedule\<TSaga\>)**

Unschedule a message, if the message was scheduled.

```csharp
public static ExceptionActivityBinder<TSaga, TException> Unschedule<TSaga, TException>(ExceptionActivityBinder<TSaga, TException> source, Schedule<TSaga> schedule)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TSaga\>](../../masstransit-abstractions/masstransit/schedule-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>
