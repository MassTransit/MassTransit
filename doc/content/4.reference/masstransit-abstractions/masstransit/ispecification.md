---

title: ISpecification

---

# ISpecification

Namespace: MassTransit

A specification, that can be validated as part of a configurator, is used
 to allow nesting and chaining of specifications while ensuring that all aspects
 of the configuration are verified correct.

```csharp
public interface ISpecification
```

## Methods

### **Validate()**

Validate the specification, ensuring that a successful build will occur.

```csharp
IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
