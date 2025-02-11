---

title: MassTransitExceptionExtensions

---

# MassTransitExceptionExtensions

Namespace: MassTransit

```csharp
public static class MassTransitExceptionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MassTransitExceptionExtensions](../masstransit/masstransitexceptionextensions)

## Methods

### **ThrowIfContainsFailure(IEnumerable\<ValidationResult\>, String)**

Compiles the validation results and throws a [ConfigurationException](../masstransit/configurationexception) if any failures are present.

```csharp
public static IReadOnlyList<ValidationResult> ThrowIfContainsFailure(IEnumerable<ValidationResult> results, string prefix)
```

#### Parameters

`results` [IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`prefix` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
An optional prefix to override the default exception prefix

#### Returns

[IReadOnlyList\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br/>

#### Exceptions

[ConfigurationException](../masstransit/configurationexception)<br/>

### **ContainsFailure(IEnumerable\<ValidationResult\>)**

```csharp
public static bool ContainsFailure(IEnumerable<ValidationResult> results)
```

#### Parameters

`results` [IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
