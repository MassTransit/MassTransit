---

title: AssemblyTypeCache

---

# AssemblyTypeCache

Namespace: MassTransit.Util

Caches assemblies and assembly types to avoid repeated assembly scanning

```csharp
public static class AssemblyTypeCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AssemblyTypeCache](../masstransit-util/assemblytypecache)

## Methods

### **Clear()**

Remove all cached assemblies, essentially forcing a reload of any new assembly scans

```csharp
public static void Clear()
```

### **ThrowIfAnyTypeScanFailures()**

Use to assert that there were no failures in type scanning when trying to find the exported types
 from any Assembly

```csharp
public static void ThrowIfAnyTypeScanFailures()
```

### **FailedAssemblies()**

```csharp
public static IEnumerable<AssemblyScanTypeInfo> FailedAssemblies()
```

#### Returns

[IEnumerable\<AssemblyScanTypeInfo\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **ForAssembly(Assembly)**

```csharp
public static Task<AssemblyScanTypeInfo> ForAssembly(Assembly assembly)
```

#### Parameters

`assembly` [Assembly](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)<br/>

#### Returns

[Task\<AssemblyScanTypeInfo\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **FindTypes(IEnumerable\<Assembly\>, Func\<Type, Boolean\>)**

```csharp
public static Task<TypeSet> FindTypes(IEnumerable<Assembly> assemblies, Func<Type, bool> filter)
```

#### Parameters

`assemblies` [IEnumerable\<Assembly\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Task\<TypeSet\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **FindTypes(IEnumerable\<Assembly\>, TypeClassification, Func\<Type, Boolean\>)**

```csharp
public static Task<IEnumerable<Type>> FindTypes(IEnumerable<Assembly> assemblies, TypeClassification classification, Func<Type, bool> filter)
```

#### Parameters

`assemblies` [IEnumerable\<Assembly\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`classification` [TypeClassification](../masstransit-util/typeclassification)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Task\<IEnumerable\<Type\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **FindTypes(Assembly, TypeClassification, Func\<Type, Boolean\>)**

```csharp
public static Task<IEnumerable<Type>> FindTypes(Assembly assembly, TypeClassification classification, Func<Type, bool> filter)
```

#### Parameters

`assembly` [Assembly](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)<br/>

`classification` [TypeClassification](../masstransit-util/typeclassification)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Task\<IEnumerable\<Type\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **FindTypesInNamespace(Type, Func\<Type, Boolean\>, TypeClassification)**

```csharp
public static IEnumerable<Type> FindTypesInNamespace(Type type, Func<Type, bool> typeFilter, TypeClassification typeClassification)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`typeFilter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`typeClassification` [TypeClassification](../masstransit-util/typeclassification)<br/>

#### Returns

[IEnumerable\<Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
