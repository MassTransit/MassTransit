---

title: ISetPropertyCollection

---

# ISetPropertyCollection

Namespace: MassTransit

```csharp
public interface ISetPropertyCollection : IPropertyCollection, IReadOnlyDictionary<String, Object>, IReadOnlyCollection<KeyValuePair<String, Object>>, IEnumerable<KeyValuePair<String, Object>>, IEnumerable
```

Implements [IPropertyCollection](../masstransit/ipropertycollection), [IReadOnlyDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2), [IReadOnlyCollection\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1), [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Methods

### **Set(String, String)**

Sets a property

```csharp
ISetPropertyCollection Set(string key, string value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The new value, or null to remove the property

#### Returns

[ISetPropertyCollection](../masstransit/isetpropertycollection)<br/>

#### Exceptions

[ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br/>

### **Set(String, Object, Boolean)**

Sets a property, overwriting an existing value if  is true

```csharp
ISetPropertyCollection Set(string key, object value, bool overwrite)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The new value, or null to remove the property

`overwrite` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[ISetPropertyCollection](../masstransit/isetpropertycollection)<br/>

#### Exceptions

[ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br/>

### **SetMany(IEnumerable\<KeyValuePair\<String, Object\>\>, Boolean)**

Set multiple properties from an existing collection, any null values a removed from the property collection

```csharp
ISetPropertyCollection SetMany(IEnumerable<KeyValuePair<string, object>> properties, bool overwrite)
```

#### Parameters

`properties` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`overwrite` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[ISetPropertyCollection](../masstransit/isetpropertycollection)<br/>
