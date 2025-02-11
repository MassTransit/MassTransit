---

title: ScheduleDateTimeExtensions

---

# ScheduleDateTimeExtensions

Namespace: MassTransit

```csharp
public static class ScheduleDateTimeExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduleDateTimeExtensions](../masstransit/scheduledatetimeextensions)

## Methods

### **Schedule\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, Schedule\<TInstance, TMessage\>, TMessage, ScheduleTimeProvider\<TInstance\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(EventActivityBinder<TInstance> source, Schedule<TInstance, TMessage> schedule, TMessage message, ScheduleTimeProvider<TInstance> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` TMessage<br/>

`timeProvider` [ScheduleTimeProvider\<TInstance\>](../../masstransit-abstractions/masstransit/scheduletimeprovider-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, Schedule\<TInstance, TMessage\>, Task\<TMessage\>, ScheduleTimeProvider\<TInstance\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(EventActivityBinder<TInstance> source, Schedule<TInstance, TMessage> schedule, Task<TMessage> message, ScheduleTimeProvider<TInstance> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`timeProvider` [ScheduleTimeProvider\<TInstance\>](../../masstransit-abstractions/masstransit/scheduletimeprovider-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, Schedule\<TInstance, TMessage\>, EventMessageFactory\<TInstance, TMessage\>, ScheduleTimeProvider\<TInstance\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(EventActivityBinder<TInstance> source, Schedule<TInstance, TMessage> schedule, EventMessageFactory<TInstance, TMessage> messageFactory, ScheduleTimeProvider<TInstance> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [EventMessageFactory\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

`timeProvider` [ScheduleTimeProvider\<TInstance\>](../../masstransit-abstractions/masstransit/scheduletimeprovider-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, Schedule\<TInstance, TMessage\>, AsyncEventMessageFactory\<TInstance, TMessage\>, ScheduleTimeProvider\<TInstance\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(EventActivityBinder<TInstance> source, Schedule<TInstance, TMessage> schedule, AsyncEventMessageFactory<TInstance, TMessage> messageFactory, ScheduleTimeProvider<TInstance> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [AsyncEventMessageFactory\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

`timeProvider` [ScheduleTimeProvider\<TInstance\>](../../masstransit-abstractions/masstransit/scheduletimeprovider-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TInstance, TMessage\>(EventActivityBinder\<TInstance\>, Schedule\<TInstance, TMessage\>, Func\<BehaviorContext\<TInstance\>, Task\<SendTuple\<TMessage\>\>\>, ScheduleTimeProvider\<TInstance\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(EventActivityBinder<TInstance> source, Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance>, Task<SendTuple<TMessage>>> messageFactory, ScheduleTimeProvider<TInstance> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [Func\<BehaviorContext\<TInstance\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeProvider` [ScheduleTimeProvider\<TInstance\>](../../masstransit-abstractions/masstransit/scheduletimeprovider-1)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Schedule\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, Schedule\<TInstance, TMessage\>, TMessage, ScheduleTimeProvider\<TInstance, TData\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule, TMessage message, ScheduleTimeProvider<TInstance, TData> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` TMessage<br/>

`timeProvider` [ScheduleTimeProvider\<TInstance, TData\>](../../masstransit-abstractions/masstransit/scheduletimeprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, Schedule\<TInstance, TMessage\>, Task\<TMessage\>, ScheduleTimeProvider\<TInstance, TData\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule, Task<TMessage> message, ScheduleTimeProvider<TInstance, TData> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`timeProvider` [ScheduleTimeProvider\<TInstance, TData\>](../../masstransit-abstractions/masstransit/scheduletimeprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, Schedule\<TInstance, TMessage\>, EventMessageFactory\<TInstance, TData, TMessage\>, ScheduleTimeProvider\<TInstance, TData\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule, EventMessageFactory<TInstance, TData, TMessage> messageFactory, ScheduleTimeProvider<TInstance, TData> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [EventMessageFactory\<TInstance, TData, TMessage\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

`timeProvider` [ScheduleTimeProvider\<TInstance, TData\>](../../masstransit-abstractions/masstransit/scheduletimeprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, Schedule\<TInstance, TMessage\>, AsyncEventMessageFactory\<TInstance, TData, TMessage\>, ScheduleTimeProvider\<TInstance, TData\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule, AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, ScheduleTimeProvider<TInstance, TData> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [AsyncEventMessageFactory\<TInstance, TData, TMessage\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

`timeProvider` [ScheduleTimeProvider\<TInstance, TData\>](../../masstransit-abstractions/masstransit/scheduletimeprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TInstance, TData, TMessage\>(EventActivityBinder\<TInstance, TData\>, Schedule\<TInstance, TMessage\>, Func\<BehaviorContext\<TInstance, TData\>, Task\<SendTuple\<TMessage\>\>\>, ScheduleTimeProvider\<TInstance, TData\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TMessage>>> messageFactory, ScheduleTimeProvider<TInstance, TData> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [Func\<BehaviorContext\<TInstance, TData\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeProvider` [ScheduleTimeProvider\<TInstance, TData\>](../../masstransit-abstractions/masstransit/scheduletimeprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Schedule\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, Schedule\<TInstance, TMessage\>, TMessage, ScheduleTimeExceptionProvider\<TInstance, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule, TMessage message, ScheduleTimeExceptionProvider<TInstance, TException> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` TMessage<br/>

`timeProvider` [ScheduleTimeExceptionProvider\<TInstance, TException\>](../../masstransit-abstractions/masstransit/scheduletimeexceptionprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, Schedule\<TInstance, TMessage\>, Task\<TMessage\>, ScheduleTimeExceptionProvider\<TInstance, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule, Task<TMessage> message, ScheduleTimeExceptionProvider<TInstance, TException> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`timeProvider` [ScheduleTimeExceptionProvider\<TInstance, TException\>](../../masstransit-abstractions/masstransit/scheduletimeexceptionprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, Schedule\<TInstance, TMessage\>, EventExceptionMessageFactory\<TInstance, TException, TMessage\>, ScheduleTimeExceptionProvider\<TInstance, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule, EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, ScheduleTimeExceptionProvider<TInstance, TException> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [EventExceptionMessageFactory\<TInstance, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-3)<br/>

`timeProvider` [ScheduleTimeExceptionProvider\<TInstance, TException\>](../../masstransit-abstractions/masstransit/scheduletimeexceptionprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, Schedule\<TInstance, TMessage\>, AsyncEventExceptionMessageFactory\<TInstance, TException, TMessage\>, ScheduleTimeExceptionProvider\<TInstance, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule, AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, ScheduleTimeExceptionProvider<TInstance, TException> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TInstance, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-3)<br/>

`timeProvider` [ScheduleTimeExceptionProvider\<TInstance, TException\>](../../masstransit-abstractions/masstransit/scheduletimeexceptionprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TInstance, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TException\>, Schedule\<TInstance, TMessage\>, Func\<BehaviorExceptionContext\<TInstance, TException\>, Task\<SendTuple\<TMessage\>\>\>, ScheduleTimeExceptionProvider\<TInstance, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule, Func<BehaviorExceptionContext<TInstance, TException>, Task<SendTuple<TMessage>>> messageFactory, ScheduleTimeExceptionProvider<TInstance, TException> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TInstance, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeProvider` [ScheduleTimeExceptionProvider\<TInstance, TException\>](../../masstransit-abstractions/masstransit/scheduletimeexceptionprovider-2)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Schedule\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Schedule\<TInstance, TMessage\>, TMessage, ScheduleTimeExceptionProvider\<TInstance, TData, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, TMessage message, ScheduleTimeExceptionProvider<TInstance, TData, TException> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` TMessage<br/>

`timeProvider` [ScheduleTimeExceptionProvider\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/scheduletimeexceptionprovider-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Schedule\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Schedule\<TInstance, TMessage\>, Task\<TMessage\>, ScheduleTimeExceptionProvider\<TInstance, TData, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, Task<TMessage> message, ScheduleTimeExceptionProvider<TInstance, TData, TException> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`message` [Task\<TMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`timeProvider` [ScheduleTimeExceptionProvider\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/scheduletimeexceptionprovider-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Schedule\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Schedule\<TInstance, TMessage\>, EventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>, ScheduleTimeExceptionProvider\<TInstance, TData, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, ScheduleTimeExceptionProvider<TInstance, TData, TException> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [EventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/eventexceptionmessagefactory-4)<br/>

`timeProvider` [ScheduleTimeExceptionProvider\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/scheduletimeexceptionprovider-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Schedule\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Schedule\<TInstance, TMessage\>, AsyncEventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>, ScheduleTimeExceptionProvider\<TInstance, TData, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, ScheduleTimeExceptionProvider<TInstance, TData, TException> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [AsyncEventExceptionMessageFactory\<TInstance, TData, TException, TMessage\>](../../masstransit-abstractions/masstransit/asynceventexceptionmessagefactory-4)<br/>

`timeProvider` [ScheduleTimeExceptionProvider\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/scheduletimeexceptionprovider-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Schedule\<TInstance, TData, TException, TMessage\>(ExceptionActivityBinder\<TInstance, TData, TException\>, Schedule\<TInstance, TMessage\>, Func\<BehaviorExceptionContext\<TInstance, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>, ScheduleTimeExceptionProvider\<TInstance, TData, TException\>, Action\<SendContext\<TMessage\>\>)**

```csharp
public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, Func<BehaviorExceptionContext<TInstance, TData, TException>, Task<SendTuple<TMessage>>> messageFactory, ScheduleTimeExceptionProvider<TInstance, TData, TException> timeProvider, Action<SendContext<TMessage>> callback)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`schedule` [Schedule\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`messageFactory` [Func\<BehaviorExceptionContext\<TInstance, TData, TException\>, Task\<SendTuple\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeProvider` [ScheduleTimeExceptionProvider\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/scheduletimeexceptionprovider-3)<br/>

`callback` [Action\<SendContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
