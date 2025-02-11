---

title: ScheduleTimeExceptionProvider<TSaga, TException>

---

# ScheduleTimeExceptionProvider\<TSaga, TException\>

Namespace: MassTransit

```csharp
public sealed class ScheduleTimeExceptionProvider<TSaga, TException> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [ScheduleTimeExceptionProvider\<TSaga, TException\>](../masstransit/scheduletimeexceptionprovider-2)<br/>
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

### **ScheduleTimeExceptionProvider(Object, IntPtr)**

```csharp
public ScheduleTimeExceptionProvider(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(BehaviorExceptionContext\<TSaga, TException\>)**

```csharp
public DateTime Invoke(BehaviorExceptionContext<TSaga, TException> context)
```

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TException\>](../masstransit/behaviorexceptioncontext-2)<br/>

#### Returns

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **BeginInvoke(BehaviorExceptionContext\<TSaga, TException\>, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(BehaviorExceptionContext<TSaga, TException> context, AsyncCallback callback, object object)
```

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TException\>](../masstransit/behaviorexceptioncontext-2)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public DateTime EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
