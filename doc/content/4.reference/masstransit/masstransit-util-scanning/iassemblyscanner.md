---

title: IAssemblyScanner

---

# IAssemblyScanner

Namespace: MassTransit.Util.Scanning

```csharp
public interface IAssemblyScanner
```

## Properties

### **Description**

Optional user-supplied diagnostic description of this scanning operation

```csharp
public abstract string Description { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Assembly(Assembly)**

Add an Assembly to the scanning operation

```csharp
void Assembly(Assembly assembly)
```

#### Parameters

`assembly` [Assembly](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)<br/>

### **Assembly(String)**

Add an Assembly by name to the scanning operation

```csharp
void Assembly(string assemblyName)
```

#### Parameters

`assemblyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AssemblyContainingType\<T\>()**

Add the Assembly that contains type T to the scanning operation

```csharp
void AssemblyContainingType<T>()
```

#### Type Parameters

`T`<br/>

### **AssemblyContainingType(Type)**

Add the Assembly that contains type to the scanning operation

```csharp
void AssemblyContainingType(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Exclude(Func\<Type, Boolean\>)**

Exclude types that match the Predicate from being scanned

```csharp
void Exclude(Func<Type, bool> exclude)
```

#### Parameters

`exclude` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **ExcludeNamespace(String)**

Exclude all types in this nameSpace or its children from the scanning operation

```csharp
void ExcludeNamespace(string nameSpace)
```

#### Parameters

`nameSpace` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ExcludeNamespaceContainingType\<T\>()**

Exclude all types in this nameSpace or its children from the scanning operation

```csharp
void ExcludeNamespaceContainingType<T>()
```

#### Type Parameters

`T`<br/>

### **Include(Func\<Type, Boolean\>)**

Only include types matching the Predicate in the scanning operation. You can
 use multiple Include() calls in a single scanning operation

```csharp
void Include(Func<Type, bool> predicate)
```

#### Parameters

`predicate` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **IncludeNamespace(String)**

Only include types from this nameSpace or its children in the scanning operation. You can
 use multiple Include() calls in a single scanning operation

```csharp
void IncludeNamespace(string nameSpace)
```

#### Parameters

`nameSpace` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IncludeNamespaceContainingType\<T\>()**

Only include types from this nameSpace or its children in the scanning operation. You can
 use multiple Include() calls in a single scanning operation

```csharp
void IncludeNamespaceContainingType<T>()
```

#### Type Parameters

`T`<br/>

### **ExcludeType\<T\>()**

Exclude this specific type from the scanning operation

```csharp
void ExcludeType<T>()
```

#### Type Parameters

`T`<br/>

### **TheCallingAssembly()**

```csharp
void TheCallingAssembly()
```

### **AssembliesFromApplicationBaseDirectory()**

```csharp
void AssembliesFromApplicationBaseDirectory()
```

### **AssembliesAndExecutablesFromPath(String)**

```csharp
void AssembliesAndExecutablesFromPath(string path)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AssembliesFromPath(String)**

```csharp
void AssembliesFromPath(string path)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AssembliesAndExecutablesFromPath(String, Func\<Assembly, Boolean\>)**

```csharp
void AssembliesAndExecutablesFromPath(string path, Func<Assembly, bool> assemblyFilter)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`assemblyFilter` [Func\<Assembly, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AssembliesFromPath(String, Func\<Assembly, Boolean\>)**

```csharp
void AssembliesFromPath(string path, Func<Assembly, bool> assemblyFilter)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`assemblyFilter` [Func\<Assembly, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **ExcludeFileNameStartsWith(String[])**

```csharp
void ExcludeFileNameStartsWith(String[] startsWith)
```

#### Parameters

`startsWith` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IncludeFileNameStartsWith(String[])**

```csharp
void IncludeFileNameStartsWith(String[] startsWith)
```

#### Parameters

`startsWith` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AssembliesAndExecutablesFromApplicationBaseDirectory()**

```csharp
void AssembliesAndExecutablesFromApplicationBaseDirectory()
```

### **ScanForTypes()**

```csharp
Task<TypeSet> ScanForTypes()
```

#### Returns

[Task\<TypeSet\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
