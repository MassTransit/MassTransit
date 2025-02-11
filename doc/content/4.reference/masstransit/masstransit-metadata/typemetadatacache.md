---

title: TypeMetadataCache

---

# TypeMetadataCache

Namespace: MassTransit.Metadata

```csharp
public static class TypeMetadataCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TypeMetadataCache](../masstransit-metadata/typemetadatacache)

## Properties

### **ImplementationBuilder**

```csharp
public static IImplementationBuilder ImplementationBuilder { get; }
```

#### Property Value

[IImplementationBuilder](../masstransit-internals/iimplementationbuilder)<br/>

## Methods

### **GetImplementationType(Type)**

```csharp
public static Type GetImplementationType(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **GetShortName(Type)**

```csharp
public static string GetShortName(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetProperties(Type)**

```csharp
public static IEnumerable<PropertyInfo> GetProperties(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IEnumerable\<PropertyInfo\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **IsValidMessageType(Type)**

```csharp
public static bool IsValidMessageType(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsTemporaryMessageType(Type)**

```csharp
public static bool IsTemporaryMessageType(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetMessageTypes(Type)**

```csharp
public static Type[] GetMessageTypes(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **GetMessageTypeNames(Type)**

```csharp
public static String[] GetMessageTypeNames(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IsValidMessageDataType(Type)**

```csharp
public static bool IsValidMessageDataType(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
