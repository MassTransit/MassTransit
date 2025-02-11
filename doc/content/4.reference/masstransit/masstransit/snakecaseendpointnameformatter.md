---

title: SnakeCaseEndpointNameFormatter

---

# SnakeCaseEndpointNameFormatter

Namespace: MassTransit

Formats the endpoint name using snake case. For example,
 SubmitOrderConsumer -&gt; submit_order
 OrderState -&gt; order_state
 UpdateCustomerActivity -&gt; update_customer_execute, update_customer_compensate

```csharp
public class SnakeCaseEndpointNameFormatter : DefaultEndpointNameFormatter, IEndpointNameFormatter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DefaultEndpointNameFormatter](../masstransit/defaultendpointnameformatter) → [SnakeCaseEndpointNameFormatter](../masstransit/snakecaseendpointnameformatter)<br/>
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

### **SnakeCaseEndpointNameFormatter(Boolean)**

Snake case endpoint formatter, which uses underscores between words

```csharp
public SnakeCaseEndpointNameFormatter(bool includeNamespace)
```

#### Parameters

`includeNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, the namespace is included in the name

### **SnakeCaseEndpointNameFormatter(String, Boolean)**

Snake case endpoint formatter, which uses underscores between words

```csharp
public SnakeCaseEndpointNameFormatter(string prefix, bool includeNamespace)
```

#### Parameters

`prefix` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Prefix to start the name, should match the casing of the formatter (such as Dev or PreProd)

`includeNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, the namespace is included in the name

### **SnakeCaseEndpointNameFormatter(String)**

Snake case endpoint formatter, which uses underscores between words

```csharp
public SnakeCaseEndpointNameFormatter(string prefix)
```

#### Parameters

`prefix` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Prefix to start the name, should match the casing of the formatter (such as Dev or PreProd)

### **SnakeCaseEndpointNameFormatter(Char, String, Boolean)**

Snake case endpoint formatter, which uses underscores between words

```csharp
public SnakeCaseEndpointNameFormatter(char separator, string prefix, bool includeNamespace)
```

#### Parameters

`separator` [Char](https://learn.microsoft.com/en-us/dotnet/api/system.char)<br/>
Specify a separator other than _ to separate words

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
