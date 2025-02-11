---

title: MessageTypeCache

---

# MessageTypeCache

Namespace: MassTransit

```csharp
public static class MessageTypeCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageTypeCache](../masstransit/messagetypecache)

## Methods

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

### **InvalidMessageTypeReason(Type)**

```csharp
public static string InvalidMessageTypeReason(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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
