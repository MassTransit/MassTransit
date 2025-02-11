---

title: LogMessage<T1, T2, T3, T4>

---

# LogMessage\<T1, T2, T3, T4\>

Namespace: MassTransit.Logging

```csharp
public sealed class LogMessage<T1, T2, T3, T4> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

`T4`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [LogMessage\<T1, T2, T3, T4\>](../masstransit-logging/logmessage-4)<br/>
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

### **LogMessage(Object, IntPtr)**

```csharp
public LogMessage(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(T1, T2, T3, T4, Exception)**

```csharp
public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception exception)
```

#### Parameters

`arg1` T1<br/>

`arg2` T2<br/>

`arg3` T3<br/>

`arg4` T4<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **BeginInvoke(T1, T2, T3, T4, Exception, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception exception, AsyncCallback callback, object object)
```

#### Parameters

`arg1` T1<br/>

`arg2` T2<br/>

`arg3` T3<br/>

`arg4` T4<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public void EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>
