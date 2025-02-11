---

title: CreateCounterDelegate

---

# CreateCounterDelegate

Namespace: MassTransit.Monitoring.Performance

```csharp
public sealed class CreateCounterDelegate : MulticastDelegate, ICloneable, ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [CreateCounterDelegate](../masstransit-monitoring-performance/createcounterdelegate)<br/>
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

### **CreateCounterDelegate(Object, IntPtr)**

```csharp
public CreateCounterDelegate(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(String, String)**

```csharp
public IPerformanceCounter Invoke(string name, string instanceName)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`instanceName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IPerformanceCounter](../masstransit-monitoring-performance/iperformancecounter)<br/>

### **BeginInvoke(String, String, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(string name, string instanceName, AsyncCallback callback, object object)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`instanceName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public IPerformanceCounter EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[IPerformanceCounter](../masstransit-monitoring-performance/iperformancecounter)<br/>
