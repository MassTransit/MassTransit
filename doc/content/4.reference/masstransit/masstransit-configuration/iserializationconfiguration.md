---

title: ISerializationConfiguration

---

# ISerializationConfiguration

Namespace: MassTransit.Configuration

```csharp
public interface ISerializationConfiguration : ISpecification
```

Implements [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **DefaultContentType**

When deserializing a message, if no ContentType is present on the receive context, use this as the default

```csharp
public abstract ContentType DefaultContentType { set; }
```

#### Property Value

ContentType<br/>

### **SerializerContentType**

When serializing a message, the content type of the serializer to use

```csharp
public abstract ContentType SerializerContentType { set; }
```

#### Property Value

ContentType<br/>

## Methods

### **AddSerializer(ISerializerFactory, Boolean)**

```csharp
void AddSerializer(ISerializerFactory factory, bool isSerializer)
```

#### Parameters

`factory` [ISerializerFactory](../../masstransit-abstractions/masstransit/iserializerfactory)<br/>

`isSerializer` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AddDeserializer(ISerializerFactory, Boolean)**

```csharp
void AddDeserializer(ISerializerFactory factory, bool isDefault)
```

#### Parameters

`factory` [ISerializerFactory](../../masstransit-abstractions/masstransit/iserializerfactory)<br/>

`isDefault` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Clear()**

Clear the configuration, removing all deserializers, serializers, and breaking the
 linkage to the bus serialization configuration.

```csharp
void Clear()
```

### **CreateSerializationConfiguration()**

```csharp
ISerializationConfiguration CreateSerializationConfiguration()
```

#### Returns

[ISerializationConfiguration](../masstransit-configuration/iserializationconfiguration)<br/>

### **CreateSerializerCollection()**

Compiles the configured serializers into a collection for use by the receive endpoint

```csharp
ISerialization CreateSerializerCollection()
```

#### Returns

[ISerialization](../../masstransit-abstractions/masstransit/iserialization)<br/>
