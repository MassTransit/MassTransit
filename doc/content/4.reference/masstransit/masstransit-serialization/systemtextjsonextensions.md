---

title: SystemTextJsonExtensions

---

# SystemTextJsonExtensions

Namespace: MassTransit.Serialization

```csharp
public static class SystemTextJsonExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SystemTextJsonExtensions](../masstransit-serialization/systemtextjsonextensions)

## Methods

### **GetObject\<T\>(JsonElement, JsonSerializerOptions)**

```csharp
public static T GetObject<T>(JsonElement jsonElement, JsonSerializerOptions options)
```

#### Type Parameters

`T`<br/>

#### Parameters

`jsonElement` JsonElement<br/>

`options` JsonSerializerOptions<br/>

#### Returns

T<br/>

### **Transform\<T\>(Object, JsonSerializerOptions)**

```csharp
public static T Transform<T>(object objectToTransform, JsonSerializerOptions options)
```

#### Type Parameters

`T`<br/>

#### Parameters

`objectToTransform` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`options` JsonSerializerOptions<br/>

#### Returns

T<br/>

### **Transform(Object, Type, JsonSerializerOptions)**

```csharp
public static object Transform(object objectToTransform, Type targetType, JsonSerializerOptions options)
```

#### Parameters

`objectToTransform` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`targetType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`options` JsonSerializerOptions<br/>

#### Returns

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
