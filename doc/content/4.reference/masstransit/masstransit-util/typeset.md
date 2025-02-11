---

title: TypeSet

---

# TypeSet

Namespace: MassTransit.Util

Access to a set of exported .Net Type's as defined in a scanning operation

```csharp
public class TypeSet
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TypeSet](../masstransit-util/typeset)

## Properties

### **Records**

For diagnostic purposes, explains which assemblies were
 scanned as part of this TypeSet, including failures

```csharp
public IEnumerable<AssemblyScanRecord> Records { get; }
```

#### Property Value

[IEnumerable\<AssemblyScanRecord\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Constructors

### **TypeSet(IEnumerable\<AssemblyScanTypeInfo\>, Func\<Type, Boolean\>)**

```csharp
public TypeSet(IEnumerable<AssemblyScanTypeInfo> allTypes, Func<Type, bool> filter)
```

#### Parameters

`allTypes` [IEnumerable\<AssemblyScanTypeInfo\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Methods

### **FindTypes(TypeClassification)**

Find any types in this TypeSet that match any combination of the TypeClassification enumeration values

```csharp
public IEnumerable<Type> FindTypes(TypeClassification classification)
```

#### Parameters

`classification` [TypeClassification](../masstransit-util/typeclassification)<br/>

#### Returns

[IEnumerable\<Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **AllTypes()**

Returns all the types in this TypeSet

```csharp
public IEnumerable<Type> AllTypes()
```

#### Returns

[IEnumerable\<Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
