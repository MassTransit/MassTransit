---

title: ProbeContext

---

# ProbeContext

Namespace: MassTransit

Passed to a probe site to inspect it for interesting things

```csharp
public interface ProbeContext
```

## Properties

### **CancellationToken**

If for some reason the probe is cancelled, allowing an early withdrawal

```csharp
public abstract CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Add(String, String)**

Add a key/value pair to the current probe context

```csharp
void Add(string key, string value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The key name

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The value

### **Add(String, Object)**

Add a key/value pair to the current probe context

```csharp
void Add(string key, object value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The key name

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The value

### **Set(Object)**

Add the properties of the object as key/value pairs to the current context

```csharp
void Set(object values)
```

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The object (typically anonymous with new{}

### **Set(IEnumerable\<KeyValuePair\<String, Object\>\>)**

Add the values from the enumeration as key/value pairs

```csharp
void Set(IEnumerable<KeyValuePair<string, object>> values)
```

#### Parameters

`values` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **CreateScope(String)**

```csharp
ProbeContext CreateScope(string key)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ProbeContext](../masstransit/probecontext)<br/>
