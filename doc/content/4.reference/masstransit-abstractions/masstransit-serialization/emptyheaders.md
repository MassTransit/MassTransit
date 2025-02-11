---

title: EmptyHeaders

---

# EmptyHeaders

Namespace: MassTransit.Serialization

```csharp
public class EmptyHeaders : Headers, IEnumerable<HeaderValue>, IEnumerable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EmptyHeaders](../masstransit-serialization/emptyheaders)<br/>
Implements [Headers](../masstransit/headers), [IEnumerable\<HeaderValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Fields

### **Instance**

```csharp
public static EmptyHeaders Instance;
```

## Methods

### **GetAll()**

```csharp
public IEnumerable<KeyValuePair<string, object>> GetAll()
```

#### Returns

[IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **TryGetHeader(String, Object)**

```csharp
public bool TryGetHeader(string key, out object value)
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

### **GetEnumerator()**

```csharp
public IEnumerator<HeaderValue> GetEnumerator()
```

#### Returns

[IEnumerator\<HeaderValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>
