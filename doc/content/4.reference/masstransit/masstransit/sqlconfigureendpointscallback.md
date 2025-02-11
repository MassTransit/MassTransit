---

title: SqlConfigureEndpointsCallback

---

# SqlConfigureEndpointsCallback

Namespace: MassTransit

```csharp
public sealed class SqlConfigureEndpointsCallback : MulticastDelegate, ICloneable, ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [SqlConfigureEndpointsCallback](../masstransit/sqlconfigureendpointscallback)<br/>
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

### **SqlConfigureEndpointsCallback(Object, IntPtr)**

```csharp
public SqlConfigureEndpointsCallback(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(IRegistrationContext, String, ISqlReceiveEndpointConfigurator)**

```csharp
public void Invoke(IRegistrationContext context, string queueName, ISqlReceiveEndpointConfigurator configurator)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configurator` [ISqlReceiveEndpointConfigurator](../masstransit/isqlreceiveendpointconfigurator)<br/>

### **BeginInvoke(IRegistrationContext, String, ISqlReceiveEndpointConfigurator, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(IRegistrationContext context, string queueName, ISqlReceiveEndpointConfigurator configurator, AsyncCallback callback, object object)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configurator` [ISqlReceiveEndpointConfigurator](../masstransit/isqlreceiveendpointconfigurator)<br/>

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
