---

title: RequestTimeout

---

# RequestTimeout

Namespace: MassTransit

A timeout, which can be a default (none) or a valid TimeSpan &gt; 0, includes factory methods to make it "cute"

```csharp
public struct RequestTimeout
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [RequestTimeout](../masstransit/requesttimeout)<br/>
Implements [IEquatable\<RequestTimeout\>](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **HasValue**

```csharp
public bool HasValue { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Value**



```csharp
public TimeSpan Value { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Exceptions

[InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br/>

### **None**

```csharp
public static RequestTimeout None { get; }
```

#### Property Value

[RequestTimeout](../masstransit/requesttimeout)<br/>

### **Default**

```csharp
public static RequestTimeout Default { get; }
```

#### Property Value

[RequestTimeout](../masstransit/requesttimeout)<br/>

## Methods

### **Equals(RequestTimeout)**

```csharp
public bool Equals(RequestTimeout other)
```

#### Parameters

`other` [RequestTimeout](../masstransit/requesttimeout)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Or(RequestTimeout)**

If this timeout has a value, return it, otherwise, return the other timeout

```csharp
public RequestTimeout Or(RequestTimeout other)
```

#### Parameters

`other` [RequestTimeout](../masstransit/requesttimeout)<br/>

#### Returns

[RequestTimeout](../masstransit/requesttimeout)<br/>

### **After(Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>)**

Create a timeout using optional arguments to build it up

```csharp
public static RequestTimeout After(Nullable<int> d, Nullable<int> h, Nullable<int> m, Nullable<int> s, Nullable<int> ms)
```

#### Parameters

`d` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
days

`h` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
hours

`m` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
minutes

`s` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
seconds

`ms` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
milliseconds

#### Returns

[RequestTimeout](../masstransit/requesttimeout)<br/>
The timeout value

#### Exceptions

[ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception)<br/>
