---

title: LicenseProduct

---

# LicenseProduct

Namespace: MassTransit.Licensing

```csharp
public class LicenseProduct
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LicenseProduct](../masstransit-licensing/licenseproduct)

## Properties

### **Name**

```csharp
public string Name { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Description**

```csharp
public string Description { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Expires**

```csharp
public Nullable<DateTime> Expires { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Features**

```csharp
public Dictionary<string, LicenseFeature> Features { get; set; }
```

#### Property Value

[Dictionary\<String, LicenseFeature\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

## Constructors

### **LicenseProduct()**

```csharp
public LicenseProduct()
```
