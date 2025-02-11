---

title: CamelCaseDictionaryExtensions

---

# CamelCaseDictionaryExtensions

Namespace: MassTransit.Serialization

```csharp
public static class CamelCaseDictionaryExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CamelCaseDictionaryExtensions](../masstransit-serialization/camelcasedictionaryextensions)

## Methods

### **TryGetValueCamelCase(IDictionary\<String, Object\>, String, Object)**

Converts a PascalCase key to camelCase and attempts to get the value from the dictionary

```csharp
public static bool TryGetValueCamelCase(IDictionary<string, object> dictionary, string key, out object value)
```

#### Parameters

`dictionary` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetValueCamelCase(IReadOnlyDictionary\<String, Object\>, String, Object)**

Converts a PascalCase key to camelCase and attempts to get the value from the dictionary

```csharp
public static bool TryGetValueCamelCase(IReadOnlyDictionary<string, object> dictionary, string key, out object value)
```

#### Parameters

`dictionary` [IReadOnlyDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
