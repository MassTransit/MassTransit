---

title: KebabCaseEndpointNameFormatter

---

# KebabCaseEndpointNameFormatter

Namespace: MassTransit

Formats the endpoint names using kebab-case (dashed snake case)
 SubmitOrderConsumer -&gt; submit-order
 OrderState -&gt; order-state
 UpdateCustomerActivity -&gt; update-customer-execute, update-customer-compensate

```csharp
public class KebabCaseEndpointNameFormatter : SnakeCaseEndpointNameFormatter, IEndpointNameFormatter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DefaultEndpointNameFormatter](../masstransit/defaultendpointnameformatter) → [SnakeCaseEndpointNameFormatter](../masstransit/snakecaseendpointnameformatter) → [KebabCaseEndpointNameFormatter](../masstransit/kebabcaseendpointnameformatter)<br/>
Implements [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)

## Properties

### **Instance**

```csharp
public static IEndpointNameFormatter Instance { get; }
```

#### Property Value

[IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

### **Separator**

```csharp
public string Separator { get; protected set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **KebabCaseEndpointNameFormatter(Boolean)**

Kebab case endpoint formatter, which uses dashes between words

```csharp
public KebabCaseEndpointNameFormatter(bool includeNamespace)
```

#### Parameters

`includeNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, the namespace is included in the name

### **KebabCaseEndpointNameFormatter(String)**

Kebab case endpoint formatter, which uses dashes between words

```csharp
public KebabCaseEndpointNameFormatter(string prefix)
```

#### Parameters

`prefix` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Prefix to start the name, should match the casing of the formatter (such as Dev or PreProd)

### **KebabCaseEndpointNameFormatter(String, Boolean)**

Kebab case endpoint formatter, which uses dashes between words

```csharp
public KebabCaseEndpointNameFormatter(string prefix, bool includeNamespace)
```

#### Parameters

`prefix` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Prefix to start the name, should match the casing of the formatter (such as Dev or PreProd)

`includeNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, the namespace is included in the name

## Methods

### **SanitizeName(String)**

```csharp
public string SanitizeName(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
