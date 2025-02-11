---

title: ScheduleDelayProvider<TSaga, TMessage>

---

# ScheduleDelayProvider\<TSaga, TMessage\>

Namespace: MassTransit

```csharp
public sealed class ScheduleDelayProvider<TSaga, TMessage> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [ScheduleDelayProvider\<TSaga, TMessage\>](../masstransit/scheduledelayprovider-2)<br/>
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

### **ScheduleDelayProvider(Object, IntPtr)**

```csharp
public ScheduleDelayProvider(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(BehaviorContext\<TSaga, TMessage\>)**

```csharp
public TimeSpan Invoke(BehaviorContext<TSaga, TMessage> context)
```

#### Parameters

`context` [BehaviorContext\<TSaga, TMessage\>](../masstransit/behaviorcontext-2)<br/>

#### Returns

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **BeginInvoke(BehaviorContext\<TSaga, TMessage\>, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(BehaviorContext<TSaga, TMessage> context, AsyncCallback callback, object object)
```

#### Parameters

`context` [BehaviorContext\<TSaga, TMessage\>](../masstransit/behaviorcontext-2)<br/>

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
