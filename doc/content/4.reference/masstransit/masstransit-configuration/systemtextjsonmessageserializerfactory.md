---

title: SystemTextJsonMessageSerializerFactory

---

# SystemTextJsonMessageSerializerFactory

Namespace: MassTransit.Configuration

```csharp
public class SystemTextJsonMessageSerializerFactory : ISerializerFactory
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SystemTextJsonMessageSerializerFactory](../masstransit-configuration/systemtextjsonmessageserializerfactory)<br/>
Implements [ISerializerFactory](../../masstransit-abstractions/masstransit/iserializerfactory)

## Properties

### **ContentType**

```csharp
public ContentType ContentType { get; }
```

#### Property Value

ContentType<br/>

## Constructors

### **SystemTextJsonMessageSerializerFactory()**

```csharp
public SystemTextJsonMessageSerializerFactory()
```

## Methods

### **CreateSerializer()**

```csharp
public IMessageSerializer CreateSerializer()
```

#### Returns

[IMessageSerializer](../../masstransit-abstractions/masstransit/imessageserializer)<br/>

### **CreateDeserializer()**

```csharp
public IMessageDeserializer CreateDeserializer()
```

#### Returns

[IMessageDeserializer](../../masstransit-abstractions/masstransit/imessagedeserializer)<br/>
