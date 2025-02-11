---

title: SerializerContextExtensions

---

# SerializerContextExtensions

Namespace: MassTransit

```csharp
public static class SerializerContextExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SerializerContextExtensions](../masstransit/serializercontextextensions)

## Methods

### **GetValue\<T\>(IObjectDeserializer, IReadOnlyDictionary\<String, Object\>, String, T)**

```csharp
public static T GetValue<T>(IObjectDeserializer context, IReadOnlyDictionary<string, object> dictionary, string key, T defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [IObjectDeserializer](../masstransit/iobjectdeserializer)<br/>

`dictionary` [IReadOnlyDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` T<br/>

#### Returns

T<br/>

### **GetValue\<T\>(IObjectDeserializer, IReadOnlyDictionary\<String, Object\>, String, Nullable\<T\>)**

```csharp
public static Nullable<T> GetValue<T>(IObjectDeserializer context, IReadOnlyDictionary<string, object> dictionary, string key, Nullable<T> defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [IObjectDeserializer](../masstransit/iobjectdeserializer)<br/>

`dictionary` [IReadOnlyDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetValue\<T\>(IObjectDeserializer, IDictionary\<String, Object\>, String, T)**

```csharp
public static T GetValue<T>(IObjectDeserializer context, IDictionary<string, object> dictionary, string key, T defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [IObjectDeserializer](../masstransit/iobjectdeserializer)<br/>

`dictionary` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` T<br/>

#### Returns

T<br/>

### **GetValue\<T\>(IObjectDeserializer, IDictionary\<String, Object\>, String, Nullable\<T\>)**

```csharp
public static Nullable<T> GetValue<T>(IObjectDeserializer context, IDictionary<string, object> dictionary, string key, Nullable<T> defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [IObjectDeserializer](../masstransit/iobjectdeserializer)<br/>

`dictionary` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetValue\<T\>(IObjectDeserializer, IHeaderProvider, String, T)**

```csharp
public static T GetValue<T>(IObjectDeserializer context, IHeaderProvider dictionary, string key, T defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [IObjectDeserializer](../masstransit/iobjectdeserializer)<br/>

`dictionary` [IHeaderProvider](../masstransit-transports/iheaderprovider)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` T<br/>

#### Returns

T<br/>

### **GetValue\<T\>(IObjectDeserializer, IHeaderProvider, String, Nullable\<T\>)**

```csharp
public static Nullable<T> GetValue<T>(IObjectDeserializer context, IHeaderProvider dictionary, string key, Nullable<T> defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [IObjectDeserializer](../masstransit/iobjectdeserializer)<br/>

`dictionary` [IHeaderProvider](../masstransit-transports/iheaderprovider)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **TryGetValue\<T\>(IObjectDeserializer, IDictionary\<String, Object\>, String, T)**

```csharp
public static bool TryGetValue<T>(IObjectDeserializer context, IDictionary<string, object> dictionary, string key, out T value)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [IObjectDeserializer](../masstransit/iobjectdeserializer)<br/>

`dictionary` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **SerializeDictionary(IObjectDeserializer, IEnumerable\<KeyValuePair\<String, Object\>\>)**

```csharp
public static string SerializeDictionary(IObjectDeserializer deserializer, IEnumerable<KeyValuePair<string, object>> values)
```

#### Parameters

`deserializer` [IObjectDeserializer](../masstransit/iobjectdeserializer)<br/>

`values` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **DeserializeDictionary\<TValue\>(IObjectDeserializer, String)**

```csharp
public static Dictionary<string, TValue> DeserializeDictionary<TValue>(IObjectDeserializer deserializer, string text)
```

#### Type Parameters

`TValue`<br/>

#### Parameters

`deserializer` [IObjectDeserializer](../masstransit/iobjectdeserializer)<br/>

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Dictionary\<String, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **TryGetValue\<T\>(IObjectDeserializer, IDictionary\<String, Object\>, String, Nullable\<T\>)**

```csharp
public static bool TryGetValue<T>(IObjectDeserializer context, IDictionary<string, object> dictionary, string key, out Nullable<T> value)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [IObjectDeserializer](../masstransit/iobjectdeserializer)<br/>

`dictionary` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeader\<T\>(ConsumeContext, String, T)**

```csharp
public static bool TryGetHeader<T>(ConsumeContext context, string key, out T value)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeader\<T\>(ConsumeContext, String, Nullable\<T\>)**

```csharp
public static bool TryGetHeader<T>(ConsumeContext context, string key, out Nullable<T> value)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeader\<T\>(SendContext, String, T)**

```csharp
public static bool TryGetHeader<T>(SendContext context, string key, out T value)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext](../masstransit/sendcontext)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeader\<T\>(SendContext, String, Nullable\<T\>)**

```csharp
public static bool TryGetHeader<T>(SendContext context, string key, out Nullable<T> value)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext](../masstransit/sendcontext)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetHeader(ConsumeContext, String, String)**

```csharp
public static string GetHeader(ConsumeContext context, string key, string defaultValue)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetHeader\<T\>(ConsumeContext, String, T)**

```csharp
public static T GetHeader<T>(ConsumeContext context, string key, T defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` T<br/>

#### Returns

T<br/>

### **GetHeader\<T\>(ConsumeContext, String, Nullable\<T\>)**

```csharp
public static Nullable<T> GetHeader<T>(ConsumeContext context, string key, Nullable<T> defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ToDictionary\<T\>(ConsumeContext, T)**

```csharp
public static Dictionary<string, object> ToDictionary<T>(ConsumeContext context, T value)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`value` T<br/>

#### Returns

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>
