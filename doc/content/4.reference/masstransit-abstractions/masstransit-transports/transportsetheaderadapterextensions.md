---

title: TransportSetHeaderAdapterExtensions

---

# TransportSetHeaderAdapterExtensions

Namespace: MassTransit.Transports

```csharp
public static class TransportSetHeaderAdapterExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransportSetHeaderAdapterExtensions](../masstransit-transports/transportsetheaderadapterextensions)

## Methods

### **Set\<TValueType\>(ITransportSetHeaderAdapter\<TValueType\>, IDictionary\<String, TValueType\>, String, String)**

```csharp
public static void Set<TValueType>(ITransportSetHeaderAdapter<TValueType> adapter, IDictionary<string, TValueType> dictionary, string key, string value)
```

#### Type Parameters

`TValueType`<br/>

#### Parameters

`adapter` [ITransportSetHeaderAdapter\<TValueType\>](../masstransit-transports/itransportsetheaderadapter-1)<br/>

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Set\<TValueType\>(ITransportSetHeaderAdapter\<TValueType\>, IDictionary\<String, TValueType\>, String, Uri)**

```csharp
public static void Set<TValueType>(ITransportSetHeaderAdapter<TValueType> adapter, IDictionary<string, TValueType> dictionary, string key, Uri value)
```

#### Type Parameters

`TValueType`<br/>

#### Parameters

`adapter` [ITransportSetHeaderAdapter\<TValueType\>](../masstransit-transports/itransportsetheaderadapter-1)<br/>

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` Uri<br/>

### **Set\<TValueType\>(ITransportSetHeaderAdapter\<TValueType\>, IDictionary\<String, TValueType\>, String, Nullable\<Guid\>)**

```csharp
public static void Set<TValueType>(ITransportSetHeaderAdapter<TValueType> adapter, IDictionary<string, TValueType> dictionary, string key, Nullable<Guid> value)
```

#### Type Parameters

`TValueType`<br/>

#### Parameters

`adapter` [ITransportSetHeaderAdapter\<TValueType\>](../masstransit-transports/itransportsetheaderadapter-1)<br/>

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Set\<TValueType\>(ITransportSetHeaderAdapter\<TValueType\>, IDictionary\<String, TValueType\>, String, Nullable\<Guid\>, Func\<Guid, String\>)**

```csharp
public static void Set<TValueType>(ITransportSetHeaderAdapter<TValueType> adapter, IDictionary<string, TValueType> dictionary, string key, Nullable<Guid> value, Func<Guid, string> formatter)
```

#### Type Parameters

`TValueType`<br/>

#### Parameters

`adapter` [ITransportSetHeaderAdapter\<TValueType\>](../masstransit-transports/itransportsetheaderadapter-1)<br/>

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`formatter` [Func\<Guid, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **Set\<TValueType\>(ITransportSetHeaderAdapter\<TValueType\>, IDictionary\<String, TValueType\>, String, Nullable\<Int32\>)**

```csharp
public static void Set<TValueType>(ITransportSetHeaderAdapter<TValueType> adapter, IDictionary<string, TValueType> dictionary, string key, Nullable<int> value)
```

#### Type Parameters

`TValueType`<br/>

#### Parameters

`adapter` [ITransportSetHeaderAdapter\<TValueType\>](../masstransit-transports/itransportsetheaderadapter-1)<br/>

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Set\<TValueType\>(ITransportSetHeaderAdapter\<TValueType\>, IDictionary\<String, TValueType\>, String, Nullable\<Int32\>, Func\<Int32, String\>)**

```csharp
public static void Set<TValueType>(ITransportSetHeaderAdapter<TValueType> adapter, IDictionary<string, TValueType> dictionary, string key, Nullable<int> value, Func<int, string> formatter)
```

#### Type Parameters

`TValueType`<br/>

#### Parameters

`adapter` [ITransportSetHeaderAdapter\<TValueType\>](../masstransit-transports/itransportsetheaderadapter-1)<br/>

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`formatter` [Func\<Int32, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **Set\<TValueType\>(ITransportSetHeaderAdapter\<TValueType\>, IDictionary\<String, TValueType\>, String, Nullable\<TimeSpan\>)**

```csharp
public static void Set<TValueType>(ITransportSetHeaderAdapter<TValueType> adapter, IDictionary<string, TValueType> dictionary, string key, Nullable<TimeSpan> value)
```

#### Type Parameters

`TValueType`<br/>

#### Parameters

`adapter` [ITransportSetHeaderAdapter\<TValueType\>](../masstransit-transports/itransportsetheaderadapter-1)<br/>

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Set\<TValueType\>(ITransportSetHeaderAdapter\<TValueType\>, IDictionary\<String, TValueType\>, String, Nullable\<TimeSpan\>, Func\<TimeSpan, String\>)**

```csharp
public static void Set<TValueType>(ITransportSetHeaderAdapter<TValueType> adapter, IDictionary<string, TValueType> dictionary, string key, Nullable<TimeSpan> value, Func<TimeSpan, string> formatter)
```

#### Type Parameters

`TValueType`<br/>

#### Parameters

`adapter` [ITransportSetHeaderAdapter\<TValueType\>](../masstransit-transports/itransportsetheaderadapter-1)<br/>

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`formatter` [Func\<TimeSpan, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **Set\<TValueType\>(ITransportSetHeaderAdapter\<TValueType\>, IDictionary\<String, TValueType\>, String, Nullable\<DateTime\>)**

```csharp
public static void Set<TValueType>(ITransportSetHeaderAdapter<TValueType> adapter, IDictionary<string, TValueType> dictionary, string key, Nullable<DateTime> value)
```

#### Type Parameters

`TValueType`<br/>

#### Parameters

`adapter` [ITransportSetHeaderAdapter\<TValueType\>](../masstransit-transports/itransportsetheaderadapter-1)<br/>

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Set\<TValueType\>(ITransportSetHeaderAdapter\<TValueType\>, IDictionary\<String, TValueType\>, String, Nullable\<DateTime\>, Func\<DateTime, String\>)**

```csharp
public static void Set<TValueType>(ITransportSetHeaderAdapter<TValueType> adapter, IDictionary<string, TValueType> dictionary, string key, Nullable<DateTime> value, Func<DateTime, string> formatter)
```

#### Type Parameters

`TValueType`<br/>

#### Parameters

`adapter` [ITransportSetHeaderAdapter\<TValueType\>](../masstransit-transports/itransportsetheaderadapter-1)<br/>

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`formatter` [Func\<DateTime, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **Set\<TValueType\>(ITransportSetHeaderAdapter\<TValueType\>, IDictionary\<String, TValueType\>, IEnumerable\<HeaderValue\>)**

```csharp
public static void Set<TValueType>(ITransportSetHeaderAdapter<TValueType> adapter, IDictionary<string, TValueType> dictionary, IEnumerable<HeaderValue> headerValues)
```

#### Type Parameters

`TValueType`<br/>

#### Parameters

`adapter` [ITransportSetHeaderAdapter\<TValueType\>](../masstransit-transports/itransportsetheaderadapter-1)<br/>

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`headerValues` [IEnumerable\<HeaderValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Set(IDictionary\<String, Object\>, IEnumerable\<HeaderValue\>)**

```csharp
public static void Set(IDictionary<string, object> dictionary, IEnumerable<HeaderValue> headerValues)
```

#### Parameters

`dictionary` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`headerValues` [IEnumerable\<HeaderValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **TryGetInt(IDictionary\<String, String\>, String, Int32)**

```csharp
public static bool TryGetInt(IDictionary<string, string> dictionary, string key, out int value)
```

#### Parameters

`dictionary` [IDictionary\<String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Set(IDictionary\<String, Object\>, HeaderValue[])**

```csharp
public static void Set(IDictionary<string, object> dictionary, HeaderValue[] headerValues)
```

#### Parameters

`dictionary` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`headerValues` [HeaderValue[]](../masstransit/headervalue)<br/>

### **SetExceptionHeaders(IDictionary\<String, Object\>, ExceptionReceiveContext)**

#### Caution

Removed with change to exception headers

---

```csharp
public static void SetExceptionHeaders(IDictionary<string, object> dictionary, ExceptionReceiveContext exceptionContext)
```

#### Parameters

`dictionary` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`exceptionContext` [ExceptionReceiveContext](../masstransit/exceptionreceivecontext)<br/>

### **SetHostHeaders(IDictionary\<String, Object\>)**

#### Caution

Removed with change to exception headers

---

```csharp
public static void SetHostHeaders(IDictionary<string, object> dictionary)
```

#### Parameters

`dictionary` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
