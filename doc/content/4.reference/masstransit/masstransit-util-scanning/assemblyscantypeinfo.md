---

title: AssemblyScanTypeInfo

---

# AssemblyScanTypeInfo

Namespace: MassTransit.Util.Scanning

```csharp
public class AssemblyScanTypeInfo
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AssemblyScanTypeInfo](../masstransit-util-scanning/assemblyscantypeinfo)

## Fields

### **ClosedTypes**

```csharp
public AssemblyTypeList ClosedTypes;
```

### **OpenTypes**

```csharp
public AssemblyTypeList OpenTypes;
```

## Properties

### **Record**

```csharp
public AssemblyScanRecord Record { get; }
```

#### Property Value

[AssemblyScanRecord](../masstransit-util-scanning/assemblyscanrecord)<br/>

## Constructors

### **AssemblyScanTypeInfo(Assembly)**

```csharp
public AssemblyScanTypeInfo(Assembly assembly)
```

#### Parameters

`assembly` [Assembly](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)<br/>

### **AssemblyScanTypeInfo(String, Func\<IEnumerable\<Type\>\>)**

```csharp
public AssemblyScanTypeInfo(string name, Func<IEnumerable<Type>> source)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`source` [Func\<IEnumerable\<Type\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

## Methods

### **FindTypes(TypeClassification)**

```csharp
public IEnumerable<Type> FindTypes(TypeClassification classification)
```

#### Parameters

`classification` [TypeClassification](../masstransit-util/typeclassification)<br/>

#### Returns

[IEnumerable\<Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
