---

title: SystemTextJsonRawMessageSerializerFactory

---

# SystemTextJsonRawMessageSerializerFactory

Namespace: MassTransit.Configuration

```csharp
public class SystemTextJsonRawMessageSerializerFactory : ISerializerFactory
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SystemTextJsonRawMessageSerializerFactory](../masstransit-configuration/systemtextjsonrawmessageserializerfactory)<br/>
Implements [ISerializerFactory](../../masstransit-abstractions/masstransit/iserializerfactory)

## Properties

### **ContentType**

```csharp
public ContentType ContentType { get; }
```

#### Property Value

ContentType<br/>

## Constructors

### **SystemTextJsonRawMessageSerializerFactory(RawSerializerOptions)**

```csharp
public SystemTextJsonRawMessageSerializerFactory(RawSerializerOptions options)
```

#### Parameters

`options` [RawSerializerOptions](../masstransit/rawserializeroptions)<br/>

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
