---

title: HeaderValue

---

# HeaderValue

Namespace: MassTransit

```csharp
public struct HeaderValue
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [HeaderValue](../masstransit/headervalue)

## Fields

### **Key**

```csharp
public string Key;
```

### **Value**

```csharp
public object Value;
```

## Constructors

### **HeaderValue(String, Object)**

```csharp
public HeaderValue(string key, object value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **HeaderValue(KeyValuePair\<String, Object\>)**

```csharp
public HeaderValue(KeyValuePair<string, object> pair)
```

#### Parameters

`pair` [KeyValuePair\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2)<br/>

## Methods

### **IsStringValue(HeaderValue\<String\>)**

```csharp
public bool IsStringValue(out HeaderValue<string> result)
```

#### Parameters

`result` [HeaderValue\<String\>](../masstransit/headervalue-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsSimpleValue(HeaderValue)**

```csharp
public bool IsSimpleValue(out HeaderValue result)
```

#### Parameters

`result` [HeaderValue](../masstransit/headervalue)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsValueStringValue(String, Object, HeaderValue\<String\>)**

```csharp
internal static bool IsValueStringValue(string key, object value, out HeaderValue<string> result)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [HeaderValue\<String\>](../masstransit/headervalue-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsValueSimpleValue(String, Object, HeaderValue)**

```csharp
internal static bool IsValueSimpleValue(string key, object value, out HeaderValue result)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [HeaderValue](../masstransit/headervalue)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
