---

title: JobPropertyCollection

---

# JobPropertyCollection

Namespace: MassTransit.Serialization

```csharp
public class JobPropertyCollection : ISetPropertyCollection, IPropertyCollection, IReadOnlyDictionary<String, Object>, IReadOnlyCollection<KeyValuePair<String, Object>>, IEnumerable<KeyValuePair<String, Object>>, IEnumerable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobPropertyCollection](../masstransit-serialization/jobpropertycollection)<br/>
Implements [ISetPropertyCollection](../../masstransit-abstractions/masstransit/isetpropertycollection), [IPropertyCollection](../../masstransit-abstractions/masstransit/ipropertycollection), [IReadOnlyDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2), [IReadOnlyCollection\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1), [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Properties**

```csharp
public Dictionary<string, object> Properties { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **JobPropertyCollection()**

```csharp
public JobPropertyCollection()
```

## Methods

### **TryGet(String, Object)**

```csharp
public bool TryGet(string key, out object value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Get\<T\>(String, T)**

```csharp
public T Get<T>(string key, T defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` T<br/>

#### Returns

T<br/>

### **Get\<T\>(String, Nullable\<T\>)**

```csharp
public Nullable<T> Get<T>(string key, Nullable<T> defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Set(String, String)**

```csharp
public ISetPropertyCollection Set(string key, string value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ISetPropertyCollection](../../masstransit-abstractions/masstransit/isetpropertycollection)<br/>

### **Set(String, Object, Boolean)**

```csharp
public ISetPropertyCollection Set(string key, object value, bool overwrite)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`overwrite` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[ISetPropertyCollection](../../masstransit-abstractions/masstransit/isetpropertycollection)<br/>

### **SetMany(IEnumerable\<KeyValuePair\<String, Object\>\>, Boolean)**

```csharp
public ISetPropertyCollection SetMany(IEnumerable<KeyValuePair<string, object>> properties, bool overwrite)
```

#### Parameters

`properties` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`overwrite` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[ISetPropertyCollection](../../masstransit-abstractions/masstransit/isetpropertycollection)<br/>
