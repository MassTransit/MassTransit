---

title: Headers

---

# Headers

Namespace: MassTransit

Headers are values outside of a message body transferred with the message.

```csharp
public interface Headers : IEnumerable<HeaderValue>, IEnumerable
```

Implements [IEnumerable\<HeaderValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Methods

### **GetAll()**

Returns all available headers

```csharp
IEnumerable<KeyValuePair<string, object>> GetAll()
```

#### Returns

[IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **TryGetHeader(String, Object)**

If the specified header name is found, returns the value of the header as an object

```csharp
bool TryGetHeader(string key, out object value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The header name

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The output header value

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the header is present, otherwise false

### **Get\<T\>(String, T)**

Returns the specified header as the type, returning a default value is the header is not found

```csharp
T Get<T>(string key, T defaultValue)
```

#### Type Parameters

`T`<br/>
The result type

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The header name

`defaultValue` T<br/>
The default value of the header if not found

#### Returns

T<br/>
The header value

### **Get\<T\>(String, Nullable\<T\>)**

Returns the specified header as the type, returning a default value is the header is not found

```csharp
Nullable<T> Get<T>(string key, Nullable<T> defaultValue)
```

#### Type Parameters

`T`<br/>
The result type

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The header name

`defaultValue` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
The default value of the header if not found

#### Returns

[Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
The header value
