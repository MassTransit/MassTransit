---

title: MergeFilterContextProvider<TInput, TSplit>

---

# MergeFilterContextProvider\<TInput, TSplit\>

Namespace: MassTransit

```csharp
public sealed class MergeFilterContextProvider<TInput, TSplit> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TInput`<br/>

`TSplit`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [MergeFilterContextProvider\<TInput, TSplit\>](../masstransit/mergefiltercontextprovider-2)<br/>
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

### **MergeFilterContextProvider(Object, IntPtr)**

```csharp
public MergeFilterContextProvider(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(TInput, TSplit)**

```csharp
public TInput Invoke(TInput inputContext, TSplit context)
```

#### Parameters

`inputContext` TInput<br/>

`context` TSplit<br/>

#### Returns

TInput<br/>

### **BeginInvoke(TInput, TSplit, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(TInput inputContext, TSplit context, AsyncCallback callback, object object)
```

#### Parameters

`inputContext` TInput<br/>

`context` TSplit<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public TInput EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

TInput<br/>
