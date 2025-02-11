---

title: TransportConfiguration

---

# TransportConfiguration

Namespace: MassTransit.Configuration

```csharp
public class TransportConfiguration : ITransportConfiguration, ISpecification, ITransportConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransportConfiguration](../masstransit-configuration/transportconfiguration)<br/>
Implements [ITransportConfiguration](../masstransit-configuration/itransportconfiguration), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [ITransportConfigurator](../../masstransit-abstractions/masstransit/itransportconfigurator)

## Properties

### **Configurator**

```csharp
public ITransportConfigurator Configurator { get; }
```

#### Property Value

[ITransportConfigurator](../../masstransit-abstractions/masstransit/itransportconfigurator)<br/>

### **PrefetchCount**

```csharp
public int PrefetchCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **TransportConfiguration(ITransportConfiguration)**

```csharp
public TransportConfiguration(ITransportConfiguration parent)
```

#### Parameters

`parent` [ITransportConfiguration](../masstransit-configuration/itransportconfiguration)<br/>

### **TransportConfiguration()**

```csharp
public TransportConfiguration()
```

## Methods

### **GetConcurrentMessageLimit()**

```csharp
public int GetConcurrentMessageLimit()
```

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
