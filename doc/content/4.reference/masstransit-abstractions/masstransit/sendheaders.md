---

title: SendHeaders

---

# SendHeaders

Namespace: MassTransit

```csharp
public interface SendHeaders : Headers, IEnumerable<HeaderValue>, IEnumerable
```

Implements [Headers](../masstransit/headers), [IEnumerable\<HeaderValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Methods

### **Set(String, String)**

```csharp
void Set(string key, string value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Set(String, Object, Boolean)**

```csharp
void Set(string key, object value, bool overwrite)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`overwrite` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
