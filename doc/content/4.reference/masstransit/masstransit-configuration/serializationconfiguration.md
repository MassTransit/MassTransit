---

title: SerializationConfiguration

---

# SerializationConfiguration

Namespace: MassTransit.Configuration

```csharp
public class SerializationConfiguration : ISerializationConfiguration, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SerializationConfiguration](../masstransit-configuration/serializationconfiguration)<br/>
Implements [ISerializationConfiguration](../masstransit-configuration/iserializationconfiguration), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **DefaultContentType**

```csharp
public ContentType DefaultContentType { set; }
```

#### Property Value

ContentType<br/>

### **SerializerContentType**

```csharp
public ContentType SerializerContentType { set; }
```

#### Property Value

ContentType<br/>

## Constructors

### **SerializationConfiguration()**

```csharp
public SerializationConfiguration()
```

## Methods

### **Clear()**

```csharp
public void Clear()
```

### **AddSerializer(ISerializerFactory, Boolean)**

```csharp
public void AddSerializer(ISerializerFactory factory, bool isSerializer)
```

#### Parameters

`factory` [ISerializerFactory](../../masstransit-abstractions/masstransit/iserializerfactory)<br/>

`isSerializer` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AddDeserializer(ISerializerFactory, Boolean)**

```csharp
public void AddDeserializer(ISerializerFactory factory, bool isDefault)
```

#### Parameters

`factory` [ISerializerFactory](../../masstransit-abstractions/masstransit/iserializerfactory)<br/>

`isDefault` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CreateSerializationConfiguration()**

```csharp
public ISerializationConfiguration CreateSerializationConfiguration()
```

#### Returns

[ISerializationConfiguration](../masstransit-configuration/iserializationconfiguration)<br/>

### **CreateSerializerCollection()**

```csharp
public ISerialization CreateSerializerCollection()
```

#### Returns

[ISerialization](../../masstransit-abstractions/masstransit/iserialization)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
