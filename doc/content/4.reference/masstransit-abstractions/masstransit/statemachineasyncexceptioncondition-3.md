---

title: StateMachineAsyncExceptionCondition<TSaga, TMessage, TException>

---

# StateMachineAsyncExceptionCondition\<TSaga, TMessage, TException\>

Namespace: MassTransit

Filters activities based on the conditional statement

```csharp
public sealed class StateMachineAsyncExceptionCondition<TSaga, TMessage, TException> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [StateMachineAsyncExceptionCondition\<TSaga, TMessage, TException\>](../masstransit/statemachineasyncexceptioncondition-3)<br/>
Implements [ICloneable](https://learn.microsoft.com/en-us/dotnet/api/system.icloneable), [ISerializable](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable)

## Properties

### **Target**

```csharp
public object Target { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **Method**

```csharp
public MethodInfo Method { get; }
```

#### Property Value

[MethodInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo)<br/>

## Constructors

### **StateMachineAsyncExceptionCondition(Object, IntPtr)**

```csharp
public StateMachineAsyncExceptionCondition(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(BehaviorExceptionContext\<TSaga, TMessage, TException\>)**

```csharp
public Task<bool> Invoke(BehaviorExceptionContext<TSaga, TMessage, TException> context)
```

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TMessage, TException\>](../masstransit/behaviorexceptioncontext-3)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **BeginInvoke(BehaviorExceptionContext\<TSaga, TMessage, TException\>, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(BehaviorExceptionContext<TSaga, TMessage, TException> context, AsyncCallback callback, object object)
```

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TMessage, TException\>](../masstransit/behaviorexceptioncontext-3)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public Task<bool> EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
