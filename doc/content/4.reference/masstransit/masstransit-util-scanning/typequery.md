---

title: TypeQuery

---

# TypeQuery

Namespace: MassTransit.Util.Scanning

```csharp
public class TypeQuery
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TypeQuery](../masstransit-util-scanning/typequery)

## Fields

### **Filter**

```csharp
public Func<Type, bool> Filter;
```

## Constructors

### **TypeQuery(TypeClassification, Func\<Type, Boolean\>)**

```csharp
public TypeQuery(TypeClassification classification, Func<Type, bool> filter)
```

#### Parameters

`classification` [TypeClassification](../masstransit-util/typeclassification)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Methods

### **Find(AssemblyScanTypeInfo)**

```csharp
public IEnumerable<Type> Find(AssemblyScanTypeInfo assembly)
```

#### Parameters

`assembly` [AssemblyScanTypeInfo](../masstransit-util-scanning/assemblyscantypeinfo)<br/>

#### Returns

[IEnumerable\<Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
