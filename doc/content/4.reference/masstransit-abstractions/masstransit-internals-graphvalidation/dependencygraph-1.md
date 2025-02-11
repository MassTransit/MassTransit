---

title: DependencyGraph<T>

---

# DependencyGraph\<T\>

Namespace: MassTransit.Internals.GraphValidation

```csharp
public class DependencyGraph<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyGraph\<T\>](../masstransit-internals-graphvalidation/dependencygraph-1)

## Constructors

### **DependencyGraph(Int32)**

```csharp
public DependencyGraph(int capacity)
```

#### Parameters

`capacity` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **Add(T)**

```csharp
public void Add(T source)
```

#### Parameters

`source` T<br/>

### **Add(T, T)**

```csharp
public void Add(T source, T target)
```

#### Parameters

`source` T<br/>

`target` T<br/>

### **Add(T, IEnumerable\<T\>)**

```csharp
public void Add(T source, IEnumerable<T> targets)
```

#### Parameters

`source` T<br/>

`targets` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **GetItemsInOrder()**

```csharp
public IEnumerable<T> GetItemsInOrder()
```

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **GetItemsInOrder(T)**

```csharp
public IEnumerable<T> GetItemsInOrder(T source)
```

#### Parameters

`source` T<br/>

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **EnsureGraphIsAcyclic()**

```csharp
public void EnsureGraphIsAcyclic()
```
