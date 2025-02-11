---

title: LicenseInfo

---

# LicenseInfo

Namespace: MassTransit.Licensing

```csharp
public class LicenseInfo
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LicenseInfo](../masstransit-licensing/licenseinfo)

## Properties

### **Contact**

```csharp
public LicenseContact Contact { get; set; }
```

#### Property Value

[LicenseContact](../masstransit-licensing/licensecontact)<br/>

### **Customer**

```csharp
public LicenseCustomer Customer { get; set; }
```

#### Property Value

[LicenseCustomer](../masstransit-licensing/licensecustomer)<br/>

### **Products**

```csharp
public Dictionary<string, LicenseProduct> Products { get; set; }
```

#### Property Value

[Dictionary\<String, LicenseProduct\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **Created**

```csharp
public DateTime Created { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Expires**

```csharp
public DateTime Expires { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

## Constructors

### **LicenseInfo()**

```csharp
public LicenseInfo()
```
