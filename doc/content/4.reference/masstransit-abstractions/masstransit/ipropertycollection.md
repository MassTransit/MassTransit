---

title: IPropertyCollection

---

# IPropertyCollection

Namespace: MassTransit

```csharp
public interface IPropertyCollection : IReadOnlyDictionary<String, Object>, IReadOnlyCollection<KeyValuePair<String, Object>>, IEnumerable<KeyValuePair<String, Object>>, IEnumerable
```

Implements [IReadOnlyDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2), [IReadOnlyCollection\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1), [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Methods

### **TryGet(String, Object)**

If the specified property name is found, returns the value of the property as an object

```csharp
bool TryGet(string key, out object value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The property name

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The output property value

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the property is present, otherwise false

### **Get\<T\>(String, T)**

Returns the specified property as the type, returning a default value is the property is not found

```csharp
T Get<T>(string key, T defaultValue)
```

#### Type Parameters

`T`<br/>
The result type

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The property name

`defaultValue` T<br/>
The default value of the property if not found

#### Returns

T<br/>
The property value

### **Get\<T\>(String, Nullable\<T\>)**

Returns the specified property as the type, returning a default value is the property is not found

```csharp
Nullable<T> Get<T>(string key, Nullable<T> defaultValue)
```

#### Type Parameters

`T`<br/>
The result type

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The property name

`defaultValue` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
The default value of the property if not found

#### Returns

[Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
The property value
