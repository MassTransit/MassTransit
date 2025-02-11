---

title: AssemblyTypeList

---

# AssemblyTypeList

Namespace: MassTransit.Util.Scanning

```csharp
public class AssemblyTypeList
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AssemblyTypeList](../masstransit-util-scanning/assemblytypelist)

## Fields

### **Abstract**

```csharp
public List<Type> Abstract;
```

### **Concrete**

```csharp
public List<Type> Concrete;
```

### **Interface**

```csharp
public List<Type> Interface;
```

## Constructors

### **AssemblyTypeList()**

```csharp
public AssemblyTypeList()
```

## Methods

### **SelectTypes(TypeClassification)**

```csharp
public IEnumerable<IList<Type>> SelectTypes(TypeClassification classification)
```

#### Parameters

`classification` [TypeClassification](../masstransit-util/typeclassification)<br/>

#### Returns

[IEnumerable\<IList\<Type\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **AllTypes()**

```csharp
public IEnumerable<Type> AllTypes()
```

#### Returns

[IEnumerable\<Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Add(Type)**

```csharp
public void Add(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
