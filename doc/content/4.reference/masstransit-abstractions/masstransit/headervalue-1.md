---

title: HeaderValue<T>

---

# HeaderValue\<T\>

Namespace: MassTransit

```csharp
public struct HeaderValue<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [HeaderValue\<T\>](../masstransit/headervalue-1)

## Fields

### **Key**

```csharp
public string Key;
```

### **Value**

```csharp
public T Value;
```

## Constructors

### **HeaderValue(String, T)**

```csharp
public HeaderValue(string key, T value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` T<br/>

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
