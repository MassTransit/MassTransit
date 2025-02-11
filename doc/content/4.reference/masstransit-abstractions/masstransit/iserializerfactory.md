---

title: ISerializerFactory

---

# ISerializerFactory

Namespace: MassTransit

```csharp
public interface ISerializerFactory
```

## Properties

### **ContentType**

```csharp
public abstract ContentType ContentType { get; }
```

#### Property Value

ContentType<br/>

## Methods

### **CreateSerializer()**

```csharp
IMessageSerializer CreateSerializer()
```

#### Returns

[IMessageSerializer](../masstransit/imessageserializer)<br/>

### **CreateDeserializer()**

```csharp
IMessageDeserializer CreateDeserializer()
```

#### Returns

[IMessageDeserializer](../masstransit/imessagedeserializer)<br/>
