---

title: ValidateMassTransitHostOptions

---

# ValidateMassTransitHostOptions

Namespace: MassTransit.DependencyInjection

```csharp
public class ValidateMassTransitHostOptions : IValidateOptions<MassTransitHostOptions>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValidateMassTransitHostOptions](../masstransit-dependencyinjection/validatemasstransithostoptions)<br/>
Implements IValidateOptions\<MassTransitHostOptions\>

## Constructors

### **ValidateMassTransitHostOptions()**

```csharp
public ValidateMassTransitHostOptions()
```

## Methods

### **Validate(String, MassTransitHostOptions)**

```csharp
public ValidateOptionsResult Validate(string name, MassTransitHostOptions options)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`options` [MassTransitHostOptions](../../masstransit-abstractions/masstransit/masstransithostoptions)<br/>

#### Returns

ValidateOptionsResult<br/>
