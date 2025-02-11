---

title: TypeCache

---

# TypeCache

Namespace: MassTransit

```csharp
public static class TypeCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TypeCache](../masstransit/typecache)

## Methods

### **GetOrAdd\<T\>(Type, ITypeCache\<T\>)**

```csharp
internal static void GetOrAdd<T>(Type type, ITypeCache<T> typeCache)
```

#### Type Parameters

`T`<br/>

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`typeCache` [ITypeCache\<T\>](../masstransit-internals/itypecache-1)<br/>

### **GetShortName(Type)**

```csharp
public static string GetShortName(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
