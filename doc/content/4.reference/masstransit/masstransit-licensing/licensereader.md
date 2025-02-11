---

title: LicenseReader

---

# LicenseReader

Namespace: MassTransit.Licensing

```csharp
public class LicenseReader
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LicenseReader](../masstransit-licensing/licensereader)

## Constructors

### **LicenseReader()**

```csharp
public LicenseReader()
```

## Methods

### **LoadFromFile(String)**

```csharp
public static LicenseInfo LoadFromFile(string path)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[LicenseInfo](../../masstransit-abstractions/masstransit-licensing/licenseinfo)<br/>

### **Load(String)**

```csharp
public static LicenseInfo Load(string license)
```

#### Parameters

`license` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[LicenseInfo](../../masstransit-abstractions/masstransit-licensing/licenseinfo)<br/>
