---

title: LambdaEqualityComparer<T>

---

# LambdaEqualityComparer\<T\>

Namespace: MassTransit.Util

```csharp
public class LambdaEqualityComparer<T> : IEqualityComparer<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LambdaEqualityComparer\<T\>](../masstransit-util/lambdaequalitycomparer-1)<br/>
Implements [IEqualityComparer\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)

## Constructors

### **LambdaEqualityComparer(Func\<T, T, Boolean\>)**

```csharp
public LambdaEqualityComparer(Func<T, T, bool> comparer)
```

#### Parameters

`comparer` [Func\<T, T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

### **LambdaEqualityComparer(Func\<T, T, Boolean\>, Func\<T, Int32\>)**

```csharp
public LambdaEqualityComparer(Func<T, T, bool> comparer, Func<T, int> hash)
```

#### Parameters

`comparer` [Func\<T, T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`hash` [Func\<T, Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Methods

### **Equals(T, T)**

```csharp
public bool Equals(T x, T y)
```

#### Parameters

`x` T<br/>

`y` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetHashCode(T)**

```csharp
public int GetHashCode(T obj)
```

#### Parameters

`obj` T<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
