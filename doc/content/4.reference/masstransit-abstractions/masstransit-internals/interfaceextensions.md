---

title: InterfaceExtensions

---

# InterfaceExtensions

Namespace: MassTransit.Internals

```csharp
public static class InterfaceExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InterfaceExtensions](../masstransit-internals/interfaceextensions)

## Methods

### **HasInterface\<T\>(Type)**

```csharp
public static bool HasInterface<T>(Type type)
```

#### Type Parameters

`T`<br/>

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **HasInterface(Type, Type)**

```csharp
public static bool HasInterface(Type type, Type interfaceType)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`interfaceType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetInterface\<T\>(Type)**

```csharp
public static Type GetInterface<T>(Type type)
```

#### Type Parameters

`T`<br/>

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **GetInterface(Type, Type)**

```csharp
public static Type GetInterface(Type type, Type interfaceType)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`interfaceType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **IsTask(Type, Type)**

```csharp
public static bool IsTask(Type type, out Type taskType)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`taskType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ClosesType(Type, Type)**

```csharp
public static bool ClosesType(Type type, Type openType)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`openType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ClosesType(Type, Type, Type[])**

```csharp
public static bool ClosesType(Type type, Type openType, out Type[] arguments)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`openType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`arguments` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ClosesType(Type, Type, Type)**

```csharp
public static bool ClosesType(Type type, Type openType, out Type closedType)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`openType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`closedType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetClosingArgument(Type, Type)**

```csharp
public static Type GetClosingArgument(Type type, Type openType)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`openType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **GetClosingArguments(Type, Type)**

```csharp
public static IEnumerable<Type> GetClosingArguments(Type type, Type openType)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`openType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IEnumerable\<Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
