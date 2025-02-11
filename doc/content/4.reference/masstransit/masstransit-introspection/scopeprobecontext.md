---

title: ScopeProbeContext

---

# ScopeProbeContext

Namespace: MassTransit.Introspection

```csharp
public class ScopeProbeContext : ProbeContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopeProbeContext](../masstransit-introspection/scopeprobecontext)<br/>
Implements [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)

## Methods

### **Add(String, String)**

```csharp
public void Add(string key, string value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Add(String, Object)**

```csharp
public void Add(string key, object value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **Set(Object)**

```csharp
public void Set(object values)
```

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **Set(IEnumerable\<KeyValuePair\<String, Object\>\>)**

```csharp
public void Set(IEnumerable<KeyValuePair<string, object>> values)
```

#### Parameters

`values` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **CreateScope(String)**

```csharp
public ProbeContext CreateScope(string key)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Build()**

```csharp
protected IDictionary<string, object> Build()
```

#### Returns

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
