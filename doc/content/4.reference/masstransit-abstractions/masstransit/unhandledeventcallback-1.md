---

title: UnhandledEventCallback<TSaga>

---

# UnhandledEventCallback\<TSaga\>

Namespace: MassTransit

Callback for an unhandled event in the state machine

```csharp
public sealed class UnhandledEventCallback<TSaga> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [UnhandledEventCallback\<TSaga\>](../masstransit/unhandledeventcallback-1)<br/>
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

### **UnhandledEventCallback(Object, IntPtr)**

```csharp
public UnhandledEventCallback(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(UnhandledEventContext\<TSaga\>)**

```csharp
public Task Invoke(UnhandledEventContext<TSaga> context)
```

#### Parameters

`context` [UnhandledEventContext\<TSaga\>](../masstransit/unhandledeventcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **BeginInvoke(UnhandledEventContext\<TSaga\>, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(UnhandledEventContext<TSaga> context, AsyncCallback callback, object object)
```

#### Parameters

`context` [UnhandledEventContext\<TSaga\>](../masstransit/unhandledeventcontext-1)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public Task EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
