---

title: ScheduleDelayExceptionProvider<TSaga, TMessage, TException>

---

# ScheduleDelayExceptionProvider\<TSaga, TMessage, TException\>

Namespace: MassTransit

```csharp
public sealed class ScheduleDelayExceptionProvider<TSaga, TMessage, TException> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [ScheduleDelayExceptionProvider\<TSaga, TMessage, TException\>](../masstransit/scheduledelayexceptionprovider-3)<br/>
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

### **ScheduleDelayExceptionProvider(Object, IntPtr)**

```csharp
public ScheduleDelayExceptionProvider(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(BehaviorExceptionContext\<TSaga, TMessage, TException\>)**

```csharp
public TimeSpan Invoke(BehaviorExceptionContext<TSaga, TMessage, TException> context)
```

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TMessage, TException\>](../masstransit/behaviorexceptioncontext-3)<br/>

#### Returns

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

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
public TimeSpan EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
