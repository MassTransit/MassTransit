---

title: ConsumerConventionCache

---

# ConsumerConventionCache

Namespace: MassTransit.Configuration

```csharp
public static class ConsumerConventionCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerConventionCache](../masstransit-configuration/consumerconventioncache)

## Methods

### **TryAdd\<T\>(T)**

```csharp
public static bool TryAdd<T>(T convention)
```

#### Type Parameters

`T`<br/>

#### Parameters

`convention` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Remove\<T\>()**

```csharp
public static bool Remove<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetConventions\<T\>()**

Returns the conventions registered for identifying message consumer types

```csharp
public static IEnumerable<IConsumerMessageConvention> GetConventions<T>()
```

#### Type Parameters

`T`<br/>
The consumer type

#### Returns

[IEnumerable\<IConsumerMessageConvention\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
