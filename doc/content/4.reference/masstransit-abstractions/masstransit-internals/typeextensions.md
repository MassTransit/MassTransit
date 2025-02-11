---

title: TypeExtensions

---

# TypeExtensions

Namespace: MassTransit.Internals

```csharp
public static class TypeExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TypeExtensions](../masstransit-internals/typeextensions)

## Methods

### **GetTypeName(Type)**

Returns an easy-to-read type name from the specified Type

```csharp
public static string GetTypeName(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetAllProperties(Type)**

```csharp
public static IEnumerable<PropertyInfo> GetAllProperties(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IEnumerable\<PropertyInfo\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **GetAllProperties(TypeInfo)**

```csharp
public static IEnumerable<PropertyInfo> GetAllProperties(TypeInfo typeInfo)
```

#### Parameters

`typeInfo` [TypeInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.typeinfo)<br/>

#### Returns

[IEnumerable\<PropertyInfo\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **GetAllInterfaces(Type)**

```csharp
public static IEnumerable<Type> GetAllInterfaces(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IEnumerable\<Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **GetAllStaticProperties(Type)**

```csharp
public static IEnumerable<PropertyInfo> GetAllStaticProperties(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IEnumerable\<PropertyInfo\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **GetStaticProperties(Type)**

```csharp
public static IEnumerable<PropertyInfo> GetStaticProperties(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IEnumerable\<PropertyInfo\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **IsConcrete(Type)**

Determines if a type is neither abstract nor an interface and can be constructed.

```csharp
public static bool IsConcrete(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type to check

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the type can be constructed, otherwise false.

### **IsInterfaceOrConcreteClass(Type)**

```csharp
public static bool IsInterfaceOrConcreteClass(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsConcreteAndAssignableTo(Type, Type)**

Determines if a type can be constructed, and if it can, additionally determines
 if the type can be assigned to the specified type.

```csharp
public static bool IsConcreteAndAssignableTo(Type type, Type assignableType)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type to evaluate

`assignableType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type to which the subject type should be checked against

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the type is concrete and can be assigned to the assignableType, otherwise false.

### **IsConcreteAndAssignableTo\<T\>(Type)**

Determines if a type can be constructed, and if it can, additionally determines
 if the type can be assigned to the specified type.

```csharp
public static bool IsConcreteAndAssignableTo<T>(Type type)
```

#### Type Parameters

`T`<br/>
The type to which the subject type should be checked against

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type to evaluate

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the type is concrete and can be assigned to the assignableType, otherwise false.

### **IsNullable(Type, Type)**

Determines if the type is a nullable type

```csharp
public static bool IsNullable(Type type, out Type underlyingType)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type

`underlyingType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The underlying type of the nullable

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the type can be null

### **IsOpenGeneric(Type)**

Determines if the type is an open generic with at least one unspecified generic argument

```csharp
public static bool IsOpenGeneric(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the type is an open generic

### **CanBeNull(Type)**

Determines if a type can be null

```csharp
public static bool CanBeNull(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the type can be null

### **GetAttribute\<T\>(ICustomAttributeProvider)**

Returns the first attribute of the specified type for the object specified

```csharp
public static IEnumerable<T> GetAttribute<T>(ICustomAttributeProvider provider)
```

#### Type Parameters

`T`<br/>
The type of attribute

#### Parameters

`provider` [ICustomAttributeProvider](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.icustomattributeprovider)<br/>
An attribute provider, which can be a MethodInfo, PropertyInfo, Type, etc.

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
The attribute instance if found, or null

### **HasAttribute\<T\>(ICustomAttributeProvider)**

Determines if the target has the specified attribute

```csharp
public static bool HasAttribute<T>(ICustomAttributeProvider provider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`provider` [ICustomAttributeProvider](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.icustomattributeprovider)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsAnonymousType(Type)**

Returns true if the type is an anonymous type

```csharp
public static bool IsAnonymousType(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsFSharpType(Type)**

Returns true if the type is an FSharp type (maybe?)

```csharp
public static bool IsFSharpType(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsInNamespace(Type, String)**

Returns true if the type is contained within the namespace

```csharp
public static bool IsInNamespace(Type type, string nameSpace)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`nameSpace` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsValueTypeOrObject(Type)**

True if the type is a value type, or an object type that is treated as a value by MassTransit

```csharp
public static bool IsValueTypeOrObject(Type valueType)
```

#### Parameters

`valueType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
