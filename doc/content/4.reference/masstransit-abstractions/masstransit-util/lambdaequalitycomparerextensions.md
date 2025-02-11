---

title: LambdaEqualityComparerExtensions

---

# LambdaEqualityComparerExtensions

Namespace: MassTransit.Util

```csharp
public static class LambdaEqualityComparerExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LambdaEqualityComparerExtensions](../masstransit-util/lambdaequalitycomparerextensions)

## Methods

### **Distinct\<T\>(IEnumerable\<T\>, Func\<T, T, Boolean\>)**

```csharp
public static IEnumerable<T> Distinct<T>(IEnumerable<T> source, Func<T, T, bool> comparer)
```

#### Type Parameters

`T`<br/>

#### Parameters

`source` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`comparer` [Func\<T, T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Except\<T\>(IEnumerable\<T\>, IEnumerable\<T\>, Func\<T, T, Boolean\>)**

```csharp
public static IEnumerable<T> Except<T>(IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> comparer)
```

#### Type Parameters

`T`<br/>

#### Parameters

`first` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`second` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`comparer` [Func\<T, T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
