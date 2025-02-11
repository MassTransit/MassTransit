---

title: TryPrintConstant

---

# TryPrintConstant

Namespace: MassTransit.Internals

Output the constant to C# string or should return `null`

```csharp
public sealed class TryPrintConstant : MulticastDelegate, ICloneable, ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [TryPrintConstant](../masstransit-internals/tryprintconstant)<br/>
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

### **TryPrintConstant(Object, IntPtr)**

```csharp
public TryPrintConstant(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(ConstantExpression)**

```csharp
public string Invoke(ConstantExpression e)
```

#### Parameters

`e` ConstantExpression<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **BeginInvoke(ConstantExpression, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(ConstantExpression e, AsyncCallback callback, object object)
```

#### Parameters

`e` ConstantExpression<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public string EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
