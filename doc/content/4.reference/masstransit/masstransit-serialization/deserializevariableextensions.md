---

title: DeserializeVariableExtensions

---

# DeserializeVariableExtensions

Namespace: MassTransit.Serialization

```csharp
public static class DeserializeVariableExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DeserializeVariableExtensions](../masstransit-serialization/deserializevariableextensions)

## Methods

### **TryGetValue\<T\>(IDictionary\<String, Object\>, String, T)**

Return an object from the dictionary converted to T using the message deserializer

```csharp
public static bool TryGetValue<T>(IDictionary<string, object> dictionary, string key, out T value)
```

#### Type Parameters

`T`<br/>

#### Parameters

`dictionary` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetValue\<T\>(IDictionary\<String, Object\>, String, Nullable\<T\>)**

Return an object from the dictionary converted to T using the message deserializer

```csharp
public static bool TryGetValue<T>(IDictionary<string, object> dictionary, string key, out Nullable<T> value)
```

#### Type Parameters

`T`<br/>

#### Parameters

`dictionary` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
