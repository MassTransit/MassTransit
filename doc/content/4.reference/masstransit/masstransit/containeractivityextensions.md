---

title: ContainerActivityExtensions

---

# ContainerActivityExtensions

Namespace: MassTransit

```csharp
public static class ContainerActivityExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ContainerActivityExtensions](../masstransit/containeractivityextensions)

## Methods

### **Activity\<TInstance, TData\>(EventActivityBinder\<TInstance, TData\>, Func\<IStateMachineActivitySelector\<TInstance, TData\>, EventActivityBinder\<TInstance, TData\>\>)**

Adds an activity to the state machine that is resolved from the container, rather than being initialized directly.

```csharp
public static EventActivityBinder<TInstance, TData> Activity<TInstance, TData>(EventActivityBinder<TInstance, TData> binder, Func<IStateMachineActivitySelector<TInstance, TData>, EventActivityBinder<TInstance, TData>> configure)
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

#### Parameters

`binder` [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

`configure` [Func\<IStateMachineActivitySelector\<TInstance, TData\>, EventActivityBinder\<TInstance, TData\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Activity\<TInstance\>(EventActivityBinder\<TInstance\>, Func\<IStateMachineActivitySelector\<TInstance\>, EventActivityBinder\<TInstance\>\>)**

Adds an activity to the state machine that is resolved from the container, rather than being initialized directly.

```csharp
public static EventActivityBinder<TInstance> Activity<TInstance>(EventActivityBinder<TInstance> binder, Func<IStateMachineActivitySelector<TInstance>, EventActivityBinder<TInstance>> configure)
```

#### Type Parameters

`TInstance`<br/>

#### Parameters

`binder` [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

`configure` [Func\<IStateMachineActivitySelector\<TInstance\>, EventActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **Activity\<TInstance, TException\>(ExceptionActivityBinder\<TInstance, TException\>, Func\<IStateMachineFaultedActivitySelector\<TInstance, TException\>, ExceptionActivityBinder\<TInstance, TException\>\>)**

Adds an activity to the state machine that is resolved from the container, but only handles Faulted behaviors

```csharp
public static ExceptionActivityBinder<TInstance, TException> Activity<TInstance, TException>(ExceptionActivityBinder<TInstance, TException> binder, Func<IStateMachineFaultedActivitySelector<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> configure)
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

#### Parameters

`binder` [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`configure` [Func\<IStateMachineFaultedActivitySelector\<TInstance, TException\>, ExceptionActivityBinder\<TInstance, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Activity\<TInstance, TMessage, TException\>(ExceptionActivityBinder\<TInstance, TMessage, TException\>, Func\<IStateMachineFaultedActivitySelector\<TInstance, TMessage, TException\>, ExceptionActivityBinder\<TInstance, TMessage, TException\>\>)**

Adds an activity to the state machine that is resolved from the container, but only handles Faulted behaviors

```csharp
public static ExceptionActivityBinder<TInstance, TMessage, TException> Activity<TInstance, TMessage, TException>(ExceptionActivityBinder<TInstance, TMessage, TException> binder, Func<IStateMachineFaultedActivitySelector<TInstance, TMessage, TException>, ExceptionActivityBinder<TInstance, TMessage, TException>> configure)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`binder` [ExceptionActivityBinder\<TInstance, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`configure` [Func\<IStateMachineFaultedActivitySelector\<TInstance, TMessage, TException\>, ExceptionActivityBinder\<TInstance, TMessage, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
