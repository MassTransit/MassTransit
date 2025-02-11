---

title: SystemTextMessageDataReference

---

# SystemTextMessageDataReference

Namespace: MassTransit.Serialization.JsonConverters

```csharp
public class SystemTextMessageDataReference : IMessageDataReference
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SystemTextMessageDataReference](../masstransit-serialization-jsonconverters/systemtextmessagedatareference)<br/>
Implements [IMessageDataReference](../masstransit-messagedata/imessagedatareference)

## Properties

### **Reference**

```csharp
public Uri Reference { get; set; }
```

#### Property Value

Uri<br/>

### **Text**

```csharp
public string Text { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Data**

```csharp
public Byte[] Data { get; set; }
```

#### Property Value

[Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

## Constructors

### **SystemTextMessageDataReference()**

```csharp
public SystemTextMessageDataReference()
```
