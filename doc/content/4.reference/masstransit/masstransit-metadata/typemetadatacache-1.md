---

title: TypeMetadataCache<T>

---

# TypeMetadataCache\<T\>

Namespace: MassTransit.Metadata

```csharp
public class TypeMetadataCache<T> : ITypeMetadataCache<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TypeMetadataCache\<T\>](../masstransit-metadata/typemetadatacache-1)<br/>
Implements [ITypeMetadataCache\<T\>](../masstransit-metadata/itypemetadatacache-1)

## Properties

### **ImplementationType**

```csharp
public static Type ImplementationType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **ShortName**

```csharp
public static string ShortName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **DiagnosticAddress**

```csharp
public static string DiagnosticAddress { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Properties**

```csharp
public static IEnumerable<PropertyInfo> Properties { get; }
```

#### Property Value

[IEnumerable\<PropertyInfo\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **IsValidMessageType**

```csharp
public static bool IsValidMessageType { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **InvalidMessageTypeReason**

```csharp
public static string InvalidMessageTypeReason { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IsTemporaryMessageType**

```csharp
public static bool IsTemporaryMessageType { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **MessageTypes**

```csharp
public static Type[] MessageTypes { get; }
```

#### Property Value

[Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **MessageTypeNames**

```csharp
public static String[] MessageTypeNames { get; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
