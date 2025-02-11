---

title: TransitionExtensions

---

# TransitionExtensions

Namespace: MassTransit

```csharp
public static class TransitionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransitionExtensions](../masstransit/transitionextensions)

## Methods

### **TransitionTo\<TSaga\>(EventActivityBinder\<TSaga\>, State)**

Transition the state machine to the specified state

```csharp
public static EventActivityBinder<TSaga> TransitionTo<TSaga>(EventActivityBinder<TSaga> source, State toState)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

`toState` [State](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **TransitionTo\<TSaga, TException\>(ExceptionActivityBinder\<TSaga, TException\>, State)**

Transition the state machine to the specified state in response to an exception

```csharp
public static ExceptionActivityBinder<TSaga, TException> TransitionTo<TSaga, TException>(ExceptionActivityBinder<TSaga, TException> source, State toState)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

`toState` [State](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **TransitionTo\<TSaga, TMessage\>(EventActivityBinder\<TSaga, TMessage\>, State)**

Transition the state machine to the specified state

```csharp
public static EventActivityBinder<TSaga, TMessage> TransitionTo<TSaga, TMessage>(EventActivityBinder<TSaga, TMessage> source, State toState)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

`toState` [State](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

### **TransitionTo\<TSaga, TMessage, TException\>(ExceptionActivityBinder\<TSaga, TMessage, TException\>, State)**

Transition the state machine to the specified state in response to an exception

```csharp
public static ExceptionActivityBinder<TSaga, TMessage, TException> TransitionTo<TSaga, TMessage, TException>(ExceptionActivityBinder<TSaga, TMessage, TException> source, State toState)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

`toState` [State](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Finalize\<TSaga, TMessage\>(EventActivityBinder\<TSaga, TMessage\>)**

Transition the state machine to the Final state

```csharp
public static EventActivityBinder<TSaga, TMessage> Finalize<TSaga, TMessage>(EventActivityBinder<TSaga, TMessage> source)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

#### Returns

[EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

### **Finalize\<TSaga\>(EventActivityBinder\<TSaga\>)**

Transition the state machine to the Final state

```csharp
public static EventActivityBinder<TSaga> Finalize<TSaga>(EventActivityBinder<TSaga> source)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`source` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Finalize\<TSaga, TMessage, TException\>(ExceptionActivityBinder\<TSaga, TMessage, TException\>)**

Transition the state machine to the Final state

```csharp
public static ExceptionActivityBinder<TSaga, TMessage, TException> Finalize<TSaga, TMessage, TException>(ExceptionActivityBinder<TSaga, TMessage, TException> source)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Finalize\<TSaga, TException\>(ExceptionActivityBinder\<TSaga, TException\>)**

Transition the state machine to the Final state

```csharp
public static ExceptionActivityBinder<TSaga, TException> Finalize<TSaga, TException>(ExceptionActivityBinder<TSaga, TException> source)
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

#### Parameters

`source` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>
