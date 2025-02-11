---

title: AssemblyScanner

---

# AssemblyScanner

Namespace: MassTransit.Util.Scanning

```csharp
public class AssemblyScanner : IAssemblyScanner
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AssemblyScanner](../masstransit-util-scanning/assemblyscanner)<br/>
Implements [IAssemblyScanner](../masstransit-util-scanning/iassemblyscanner)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Description**

```csharp
public string Description { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **AssemblyScanner()**

```csharp
public AssemblyScanner()
```

## Methods

### **Assembly(Assembly)**

```csharp
public void Assembly(Assembly assembly)
```

#### Parameters

`assembly` [Assembly](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)<br/>

### **Assembly(String)**

```csharp
public void Assembly(string assemblyName)
```

#### Parameters

`assemblyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AssemblyContainingType\<T\>()**

```csharp
public void AssemblyContainingType<T>()
```

#### Type Parameters

`T`<br/>

### **AssemblyContainingType(Type)**

```csharp
public void AssemblyContainingType(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Exclude(Func\<Type, Boolean\>)**

```csharp
public void Exclude(Func<Type, bool> exclude)
```

#### Parameters

`exclude` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **ExcludeNamespace(String)**

```csharp
public void ExcludeNamespace(string nameSpace)
```

#### Parameters

`nameSpace` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ExcludeNamespaceContainingType\<T\>()**

```csharp
public void ExcludeNamespaceContainingType<T>()
```

#### Type Parameters

`T`<br/>

### **Include(Func\<Type, Boolean\>)**

```csharp
public void Include(Func<Type, bool> predicate)
```

#### Parameters

`predicate` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **IncludeNamespace(String)**

```csharp
public void IncludeNamespace(string nameSpace)
```

#### Parameters

`nameSpace` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IncludeNamespaceContainingType\<T\>()**

```csharp
public void IncludeNamespaceContainingType<T>()
```

#### Type Parameters

`T`<br/>

### **ExcludeType\<T\>()**

```csharp
public void ExcludeType<T>()
```

#### Type Parameters

`T`<br/>

### **AssembliesFromApplicationBaseDirectory()**

```csharp
public void AssembliesFromApplicationBaseDirectory()
```

### **AssembliesAndExecutablesFromPath(String)**

```csharp
public void AssembliesAndExecutablesFromPath(string path)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AssembliesFromPath(String)**

```csharp
public void AssembliesFromPath(string path)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AssembliesAndExecutablesFromPath(String, Func\<Assembly, Boolean\>)**

```csharp
public void AssembliesAndExecutablesFromPath(string path, Func<Assembly, bool> assemblyFilter)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`assemblyFilter` [Func\<Assembly, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AssembliesFromPath(String, Func\<Assembly, Boolean\>)**

```csharp
public void AssembliesFromPath(string path, Func<Assembly, bool> assemblyFilter)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`assemblyFilter` [Func\<Assembly, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **ExcludeFileNameStartsWith(String[])**

```csharp
public void ExcludeFileNameStartsWith(String[] startsWith)
```

#### Parameters

`startsWith` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IncludeFileNameStartsWith(String[])**

```csharp
public void IncludeFileNameStartsWith(String[] startsWith)
```

#### Parameters

`startsWith` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AssembliesAndExecutablesFromApplicationBaseDirectory()**

```csharp
public void AssembliesAndExecutablesFromApplicationBaseDirectory()
```

### **ScanForTypes()**

```csharp
public Task<TypeSet> ScanForTypes()
```

#### Returns

[Task\<TypeSet\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **TheCallingAssembly()**

```csharp
public void TheCallingAssembly()
```

### **Contains(String)**

```csharp
public bool Contains(string assemblyName)
```

#### Parameters

`assemblyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **HasAssemblies()**

```csharp
public bool HasAssemblies()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
